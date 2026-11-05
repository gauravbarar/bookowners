using BookOwners.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookOwners.Infrastructure.Persistence;

public class BookOwnersDbContext(DbContextOptions<BookOwnersDbContext> options) : DbContext(options)
{
    public DbSet<BookOwner> BookOwners => Set<BookOwner>();

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookOwnersDbContext).Assembly);
}
