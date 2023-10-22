using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.EF.UnitOfWork;
using MediatR;

namespace FC.CodeFlix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateCategory));

        AddRepositories(services);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .AddTransient<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
