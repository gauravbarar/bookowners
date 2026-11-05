namespace BookOwners.Application.BookOwners;

public sealed record BookOwnerDto(string Name, int Age, IReadOnlyList<BookDto> Books);

public sealed record BookDto(string Name, string Type);
