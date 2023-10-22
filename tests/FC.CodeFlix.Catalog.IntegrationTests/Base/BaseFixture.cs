using Bogus;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Base;
public abstract class BaseFixture
{
    protected Faker Faker { get; set; }

    protected BaseFixture() => Faker = new Faker("pt_BR");

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
    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;

    public List<Category> GetExampleCategoriesList(int lengh = 10) =>
        Enumerable.Range(1, lengh)
        .Select(_ => GetExampleCategory()).ToList();
}
