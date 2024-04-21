using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddConnectionsDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbConnection(configuration);

        return services;
    }

    private static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:CatalogDb"];

        services.AddDbContext<CodeflixCatelogDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                b =>
                {
                    b.MigrationsAssembly("FC.Codeflix.Catalog.Api");
                    b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                }
            );
        });

        return services;
    }
}
