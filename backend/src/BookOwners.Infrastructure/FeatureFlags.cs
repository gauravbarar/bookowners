namespace BookOwners.Infrastructure;

public static class FeatureFlags
{
    /// <summary>Flag to decide whether to use BUPA API or DB. false means DB.</summary>
    public const string UseBupaApi = nameof(UseBupaApi);
}
