using BookOwners.Application.BookOwners;
using BookOwners.Infrastructure.Persistence;
using BookOwners.Infrastructure.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace BookOwners.Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringName = "BookOwners";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFeatureManagement();

        services.AddDbContext<BookOwnersDbContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>()
                .GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{ConnectionStringName}' is not configured.");

            options.UseSqlServer(connectionString);
        });

        services.AddOptions<BupaApiOptions>()
            .Bind(configuration.GetSection(BupaApiOptions.SectionName))
            .Validate(options => options.BaseUrl is not null, "BupaApi:BaseUrl is required.")
            .ValidateOnStart();

        services.AddHttpClient<BupaApiBookOwnerSource>((serviceProvider, client) =>
                client.BaseAddress = serviceProvider.GetRequiredService<IOptions<BupaApiOptions>>().Value.BaseUrl)
            .AddStandardResilienceHandler();

        services.AddScoped<DatabaseBookOwnerSource>();
        services.AddScoped<IBookOwnerSource, FeatureFlaggedBookOwnerSource>();

        return services;
    }

    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookOwnersDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
