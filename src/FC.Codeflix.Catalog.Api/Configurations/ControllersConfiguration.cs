using FC.Codeflix.Catalog.Api.Configurations.Policies;
using FC.Codeflix.Catalog.Api.Filters;
using FluentValidation;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddConfigurationController(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("*",
            policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });
        });

        ValidatorOptions.Global.LanguageManager.Enabled = false;

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
