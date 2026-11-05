namespace BookOwners.Application.BookOwners;

public interface IBookOwnerService
{
    Task<IReadOnlyList<BookOwnerDto>> GetBookOwnersAsync(CancellationToken cancellationToken);
}
