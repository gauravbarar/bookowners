using BookOwners.Application.BookOwners;

namespace BookOwners.Api.Endpoints;

internal static class BookOwnerEndpoints
{
    public static IEndpointRouteBuilder MapBookOwnerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/bookowners", GetBookOwnersAsync)
            .WithName("GetBookOwners")
            .WithTags("BookOwners")
            .Produces<IReadOnlyList<BookOwnerDto>>()
            .ProducesProblem(StatusCodes.Status502BadGateway);

        return app;
    }

    private static async Task<IResult> GetBookOwnersAsync(
        IBookOwnerService service,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await service.GetBookOwnersAsync(cancellationToken));
}
