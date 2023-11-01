using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddConnectionsDI(this IServiceCollection services)
    {
        services.AddDbConnection();

        return services;
    }

    private static IServiceCollection AddDbConnection(this IServiceCollection services)
    {
        services.AddDbContext<CodeflixCatelogDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemory-DSV-Database");
        });

        return services;
    }
}
