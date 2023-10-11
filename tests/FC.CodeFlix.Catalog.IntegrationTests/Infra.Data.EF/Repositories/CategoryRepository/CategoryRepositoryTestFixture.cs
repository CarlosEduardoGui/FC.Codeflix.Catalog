using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catelog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategotyRepositoryTestFitureCollection : ICollectionFixture<CategoryRepositoryTestFixture> { }

public class CategoryRepositoryTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[..255];

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];

        return categoryDescription;
    }

    public Category GetExampleCategory() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            );

    public List<Category> GetExampleCategoriesList(int lengh = 10) =>
        Enumerable.Range(1, lengh)
        .Select(_ => GetExampleCategory()).ToList();

    public List<Category> GetExampleCategoriesListWithNames(List<string> names) =>
    names.Select(name =>
    {
        var category = GetExampleCategory();
        category.Update(name);
        return category;
    }
    ).ToList();


    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;

    public CodeFlixCatelogDbContext CreateDbContext(bool preserveData = false)
    {
        var context = new CodeFlixCatelogDbContext(
                new DbContextOptionsBuilder<CodeFlixCatelogDbContext>()
                  .UseInMemoryDatabase("integration-tests-db")
                  .Options
            );

        if (preserveData is false)
            context.Database.EnsureDeleted();

        return context;
    }

    public List<Category> CloneCategoriesListOrdered(List<Category> categoriesList, string orderBy, SearchOrder order)
    {
        var listClone = new List<Category>(categoriesList);
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
