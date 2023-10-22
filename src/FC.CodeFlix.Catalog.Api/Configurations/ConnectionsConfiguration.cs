using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddConnectionsDI(this IServiceCollection services)
    {
        services.AddDbConnection();

        return services;
    }

    private static IServiceCollection AddDbConnection(this IServiceCollection services)
    {
        services.AddDbContext<CodeFlixCatelogDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemory-DSV-Database");
        });

        return services;
    }
}
