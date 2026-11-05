using System.Net;
using BookOwners.Infrastructure.Persistence;
using BookOwners.Infrastructure.Sources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookOwners.IntegrationTests;

public sealed class ApiFactory(bool useBupaApi = false, string upstreamBody = "[]") : WebApplicationFactory<Program>
{
    private readonly string databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("FeatureManagement:UseBupaApi", useBupaApi.ToString());

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<BookOwnersDbContext>>();
            services.AddDbContext<BookOwnersDbContext>(options => options.UseInMemoryDatabase(databaseName));

            services.AddHttpClient<BupaApiBookOwnerSource>()
                .ConfigurePrimaryHttpMessageHandler(() => new StubHandler(upstreamBody));
        });
    }

    public HttpClient CreateSeededClient()
    {
        var client = CreateClient();

        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<BookOwnersDbContext>().Database.EnsureCreated();

        return client;
    }

    private sealed class StubHandler(string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) });
    }
}
