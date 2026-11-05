using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookOwners.Infrastructure.Persistence;

public sealed class BookOwnersDbContextFactory : IDesignTimeDbContextFactory<BookOwnersDbContext>
{
    public BookOwnersDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BookOwnersDbContext>()
            .UseSqlServer("Server=design-time-only;Database=BookOwners")
            .Options;

        return new BookOwnersDbContext(options);
    }
}
