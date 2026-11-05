using BookOwners.Domain.Entities;

namespace BookOwners.Infrastructure.Persistence;

/// <summary>Extracted from the live Bupa API response.</summary>
internal static class SeedData
{
    public static readonly BookOwner[] BookOwners =
    [
        new() { Id = 1, Name = "Jane", Age = 23 },
        new() { Id = 2, Name = "Charlotte", Age = 14 },
        new() { Id = 3, Name = "Max", Age = 25 },
        new() { Id = 4, Name = "William", Age = 15 },
        new() { Id = 5, Name = "Charles", Age = 17 },
    ];

    public static readonly Book[] Books =
    [
        new() { Id = 1, BookOwnerId = 1, Name = "Hamlet", Type = BookType.Hardcover },
        new() { Id = 2, BookOwnerId = 1, Name = "Wuthering Heights", Type = BookType.Paperback },
        new() { Id = 3, BookOwnerId = 2, Name = "Hamlet", Type = BookType.Paperback },
        new() { Id = 4, BookOwnerId = 3, Name = "React: The Ultimate Guide", Type = BookType.Hardcover },
        new() { Id = 5, BookOwnerId = 3, Name = "Gulliver's Travels", Type = BookType.Hardcover },
        new() { Id = 6, BookOwnerId = 3, Name = "Jane Eyre", Type = BookType.Paperback },
        new() { Id = 7, BookOwnerId = 3, Name = "Great Expectations", Type = BookType.Hardcover },
        new() { Id = 8, BookOwnerId = 4, Name = "Great Expectations", Type = BookType.Hardcover },
        new() { Id = 9, BookOwnerId = 5, Name = "Little Red Riding Hood", Type = BookType.Hardcover },
        new() { Id = 10, BookOwnerId = 5, Name = "The Hobbit", Type = BookType.Ebook },
    ];
}
