using BookOwners.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOwners.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(book => book.Name).HasMaxLength(300);
        builder.Property(book => book.Type).HasConversion<string>().HasMaxLength(50);

        builder.HasData(SeedData.Books);
    }
}
