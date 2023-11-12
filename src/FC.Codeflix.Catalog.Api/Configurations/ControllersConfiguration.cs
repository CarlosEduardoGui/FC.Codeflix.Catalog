using FC.Codeflix.Catalog.Api.Configurations.Policies;
using FC.Codeflix.Catalog.Api.Filters;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddConfigurationController(this IServiceCollection services)
    {
        services
            .AddControllers(options => options.Filters.Add(typeof(ApiGlobalExceptionFilter)))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy();
            });

        services.AddDocumentation();
        return services;
    }

    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static WebApplication UseDocumentation(
        this WebApplication app
    )
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
