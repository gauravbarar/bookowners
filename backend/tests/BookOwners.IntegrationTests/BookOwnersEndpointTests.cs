using System.Net;
using System.Net.Http.Json;
using BookOwners.Application.BookOwners;

namespace BookOwners.IntegrationTests;

public class BookOwnersEndpointTests
{
    private const string Route = "/api/v1/bookowners";

    [Fact]
    public async Task Flag_off_returns_seeded_owners_from_the_database()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateSeededClient();

        var owners = await client.GetFromJsonAsync<List<BookOwnerDto>>(Route);

        Assert.NotNull(owners);
        Assert.Equal(5, owners.Count);
        Assert.Equal(10, owners.Sum(owner => owner.Books.Count));
    }

    [Fact]
    public async Task Flag_on_returns_owners_from_the_bupa_api()
    {
        await using var factory = new ApiFactory(
            useBupaApi: true,
            upstreamBody: """[{"name":"Stub","age":40,"books":[]}]""");
        var client = factory.CreateSeededClient();

        var owners = await client.GetFromJsonAsync<List<BookOwnerDto>>(Route);

        Assert.NotNull(owners);
        Assert.Equal("Stub", Assert.Single(owners).Name);
    }

    [Fact]
    public async Task Empty_upstream_body_returns_502_problem_details()
    {
        await using var factory = new ApiFactory(useBupaApi: true, upstreamBody: "");
        var client = factory.CreateSeededClient();

        var response = await client.GetAsync(Route);

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Health_endpoint_returns_healthy()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateSeededClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
