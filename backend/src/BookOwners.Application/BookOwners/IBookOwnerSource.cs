namespace BookOwners.Application.BookOwners;

public interface IBookOwnerSource
{
    Task<IReadOnlyList<BookOwnerDto>> GetBookOwnersAsync(CancellationToken cancellationToken);
}
