using System.Text.Json;
using BookOwners.Application.BookOwners;
using BookOwners.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Timeout;

namespace BookOwners.Infrastructure.Sources;

public sealed class BupaApiBookOwnerSource(
    HttpClient httpClient,
    IOptions<BupaApiOptions> options,
    ILogger<BupaApiBookOwnerSource> logger) : IBookOwnerSource
{
    private const string BookOwnersPath = "api/v1/bookowners";

    // Strict parsing rejects invalid/corrupt data that rather than passing.
    private static readonly JsonSerializerOptions StrictJsonOptions = new(JsonSerializerOptions.Web)
    {
        PropertyNameCaseInsensitive = false,
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true,
    };

    public async Task<IReadOnlyList<BookOwnerDto>> GetBookOwnersAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;

        // 429 and 5xx are taken cared by AddStandardResilienceHandler.
        // for bad data for json serialization issues, inplemented retry here.
        for (var attempt = 1; ; attempt++)
        {
            var body = await FetchBodyAsync(cancellationToken);

            if (Parse(body) is { } owners)
            {
                return owners;
            }

            logger.LogWarning(
                "Bupa API returned unusable data on attempt {Attempt} of {MaxAttempts}.",
                attempt,
                settings.MaxAttempts);

            if (attempt >= settings.MaxAttempts)
            {
                throw new UpstreamDataException($"Bupa API returned unusable data {attempt} times.");
            }

            await Task.Delay(settings.RetryDelay * attempt, cancellationToken);
        }
    }

    private async Task<string> FetchBodyAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(BookOwnersPath, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UpstreamDataException($"Bupa API returned status {(int)response.StatusCode}.");
            }

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new UpstreamDataException("Bupa API could not be reached.", exception);
        }
        catch (TimeoutRejectedException exception)
        {
            throw new UpstreamDataException("Bupa API timed out.", exception);
        }
    }

    private static List<BookOwnerDto>? Parse(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            var owners = JsonSerializer.Deserialize<List<BookOwnerDto>>(body, StrictJsonOptions);

            return owners is null || owners.Any(owner => owner is null) ? null : owners;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
