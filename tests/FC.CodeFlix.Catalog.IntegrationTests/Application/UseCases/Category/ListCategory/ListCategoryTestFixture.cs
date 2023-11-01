﻿using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Common;
using Xunit;
using CategoryEntity = FC.CodeFlix.Catalog.Domain.Entity.Category;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategory;

[CollectionDefinition(nameof(ListCategoryTestFixture))]
public class ListCategoryTestFixtureCollection : ICollectionFixture<ListCategoryTestFixture> { }

public class ListCategoryTestFixture : CategoryUseCaseBaseFixture
{
    public List<CategoryEntity> GetExampleCategoriesListWithNames(List<string> names) =>
    names.Select(name =>
    {
        var category = GetExampleCategory();
        category.Update(name);
        return category;
    }
    ).ToList();

    public List<CategoryEntity> CloneCategoriesListOrdered(List<CategoryEntity> categoriesList, string orderBy, SearchOrder order)
    {
        var listClone = new List<CategoryEntity>(categoriesList);
        var orderEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.ASC) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.ASC) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.ASC) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.DESC) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name),
        };

        return orderEnumerable.ToList();
    }
}