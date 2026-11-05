using BookOwners.Application.BookOwners;
using BookOwners.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookOwners.Infrastructure.Sources;

public sealed class DatabaseBookOwnerSource(BookOwnersDbContext dbContext) : IBookOwnerSource
{
    public async Task<IReadOnlyList<BookOwnerDto>> GetBookOwnersAsync(CancellationToken cancellationToken)
    {
        var owners = await dbContext.BookOwners
            .AsNoTracking()
            .Include(owner => owner.Books)
            .OrderBy(owner => owner.Id)
            .ToListAsync(cancellationToken);

        return owners
            .Select(owner => new BookOwnerDto(
                owner.Name,
                owner.Age,
                owner.Books.Select(book => new BookDto(book.Name, book.Type.ToString())).ToList()))
            .ToList();
    }
}
