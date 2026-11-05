using BookOwners.Application.BookOwners;
using Microsoft.FeatureManagement;

namespace BookOwners.Infrastructure.Sources;

public sealed class FeatureFlaggedBookOwnerSource(
    IFeatureManagerSnapshot featureManager,
    DatabaseBookOwnerSource databaseSource,
    BupaApiBookOwnerSource bupaApiSource) : IBookOwnerSource
{
    public async Task<IReadOnlyList<BookOwnerDto>> GetBookOwnersAsync(CancellationToken cancellationToken)
    {
        var useBupaApi = await featureManager.IsEnabledAsync(FeatureFlags.UseBupaApi);
        IBookOwnerSource source = useBupaApi ? bupaApiSource : databaseSource;

        return await source.GetBookOwnersAsync(cancellationToken);
    }
}
