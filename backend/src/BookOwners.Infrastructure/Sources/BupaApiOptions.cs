namespace BookOwners.Infrastructure.Sources;

public sealed class BupaApiOptions
{
    public const string SectionName = "BupaApi";

    public Uri? BaseUrl { get; set; }

    public int MaxAttempts { get; set; } = 3;

    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(200);
}
