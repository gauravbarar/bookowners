namespace BookOwners.Domain.Entities;

public class Book
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public BookType Type { get; set; }

    public int BookOwnerId { get; set; }
}
