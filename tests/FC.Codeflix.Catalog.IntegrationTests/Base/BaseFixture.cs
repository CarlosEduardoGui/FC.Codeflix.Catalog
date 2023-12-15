using Bogus;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using UoW = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork.UnitOfWork;

namespace FC.Codeflix.Catalog.IntegrationTests.Base;
public abstract class BaseFixture
{
    protected Faker Faker { get; set; }

    protected BaseFixture() => Faker = new Faker("pt_BR");

    public UoW CreateUnitOfWork(CodeflixCatelogDbContext dbContext)
        => new(dbContext);

    public CategoryRepository CreateCategoryRepository(CodeflixCatelogDbContext dbContext)
        => new(dbContext);

    public CodeflixCatelogDbContext CreateDbContext(bool preserveData = false)
    {
        var context = new CodeflixCatelogDbContext(
                new DbContextOptionsBuilder<CodeflixCatelogDbContext>()
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
