namespace BookOwners.Domain.Entities;

public class BookOwner
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int Age { get; set; }

    public List<Book> Books { get; set; } = [];
}
