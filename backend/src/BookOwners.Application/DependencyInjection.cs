using BookOwners.Application.BookOwners;
using Microsoft.Extensions.DependencyInjection;

namespace BookOwners.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookOwnerService, BookOwnerService>();
        return services;
    }
}
