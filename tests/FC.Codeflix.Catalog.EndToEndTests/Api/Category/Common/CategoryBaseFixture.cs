using FC.Codeflix.Catalog.EndToEndTests.Common;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

[CollectionDefinition(nameof(CategoryBaseFixture))]
public class CategoryBaseFixtureCollection : ICollectionFixture<CategoryBaseFixture> { }

public class CategoryBaseFixture : BaseFixture
{
    public CategoryPersistence Persistence;

    public CategoryBaseFixture() : base() =>
        Persistence = new CategoryPersistence(CreateDbContext());

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

    public CategoryEntity GetExampleCategory() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            );

    public List<CategoryEntity> GetExampleCategoriesList(int listLenght = 15)
        => Enumerable.Range(1, listLenght).Select(_ => new CategoryEntity(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            )
        ).ToList();

    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;
}
