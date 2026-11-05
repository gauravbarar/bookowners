using BookOwners.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOwners.Infrastructure.Persistence.Configurations;

internal sealed class BookOwnerConfiguration : IEntityTypeConfiguration<BookOwner>
{
    public void Configure(EntityTypeBuilder<BookOwner> builder)
    {
        builder.Property(owner => owner.Name).HasMaxLength(200);

        builder.HasMany(owner => owner.Books)
            .WithOne()
            .HasForeignKey(book => book.BookOwnerId);

        builder.HasData(SeedData.BookOwners);
    }
}
