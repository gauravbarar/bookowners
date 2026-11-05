using System.Net;
using BookOwners.Application.Exceptions;
using BookOwners.Infrastructure.Sources;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BookOwners.UnitTests;

public class BupaApiBookOwnerSourceTests
{
    private const int MaxAttempts = 3;
    private const string ValidBody = """[{"name":"Jane","age":23,"books":[{"name":"Hamlet","type":"Hardcover"}]}]""";
    private const string CorruptedBody = """[{"name":"Charles","AGE":17,"":[{"name":"The Hobbit","TYPE":"Ebook"}]}]""";

    [Fact]
    public async Task Returns_owners_from_a_valid_response()
    {
        var handler = new StubHandler((HttpStatusCode.OK, ValidBody));

        var owners = await CreateSource(handler).GetBookOwnersAsync(CancellationToken.None);

        var owner = Assert.Single(owners);
        Assert.Equal("Jane", owner.Name);
        Assert.Equal(23, owner.Age);
        Assert.Equal("Hardcover", Assert.Single(owner.Books).Type);
        Assert.Equal(1, handler.Calls);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("null")]
    [InlineData("[null]")]
    [InlineData("not json")]
    [InlineData(CorruptedBody)]
    [InlineData("""[{"name":"Jane","age":23}]""")]
    [InlineData("""[{"name":"Jane","age":23,"books":null}]""")]
    [InlineData("""[{"name":"Jane","age":23,"books":[{"name":"Hamlet"}]}]""")]
    public async Task Gives_up_after_max_attempts_when_every_body_is_unusable(string body)
    {
        var handler = new StubHandler((HttpStatusCode.OK, body));

        await Assert.ThrowsAsync<UpstreamDataException>(
            () => CreateSource(handler).GetBookOwnersAsync(CancellationToken.None));

        Assert.Equal(MaxAttempts, handler.Calls);
    }

    [Fact]
    public async Task Recovers_when_a_retry_returns_valid_data()
    {
        var handler = new StubHandler(
            (HttpStatusCode.OK, ""),
            (HttpStatusCode.OK, CorruptedBody),
            (HttpStatusCode.OK, ValidBody));

        var owners = await CreateSource(handler).GetBookOwnersAsync(CancellationToken.None);

        Assert.Equal("Jane", Assert.Single(owners).Name);
        Assert.Equal(3, handler.Calls);
    }

    [Fact]
    public async Task Does_not_add_retries_on_top_of_the_http_resilience_handler()
    {
        var handler = new StubHandler((HttpStatusCode.TooManyRequests, ""));

        await Assert.ThrowsAsync<UpstreamDataException>(
            () => CreateSource(handler).GetBookOwnersAsync(CancellationToken.None));

        Assert.Equal(1, handler.Calls);
    }

    private static BupaApiBookOwnerSource CreateSource(StubHandler handler) =>
        new(
            new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") },
            Options.Create(new BupaApiOptions { MaxAttempts = MaxAttempts, RetryDelay = TimeSpan.Zero }),
            NullLogger<BupaApiBookOwnerSource>.Instance);

    private sealed class StubHandler(params (HttpStatusCode Status, string Body)[] responses) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var (status, body) = responses[Math.Min(Calls, responses.Length - 1)];
            Calls++;

            return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body) });
        }
    }
}
