namespace BookOwners.Application.BookOwners;

public sealed class BookOwnerService(IBookOwnerSource source) : IBookOwnerService
{
    public Task<IReadOnlyList<BookOwnerDto>> GetBookOwnersAsync(CancellationToken cancellationToken) =>
        source.GetBookOwnersAsync(cancellationToken);
}
