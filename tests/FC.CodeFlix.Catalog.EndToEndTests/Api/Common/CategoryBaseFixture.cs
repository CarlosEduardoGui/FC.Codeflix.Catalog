using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.EndToEndTests.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Common;
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

    public Category GetExampleCategory() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            );

    public List<Category> GetExampleCategoriesList(int listLenght = 15)
        => Enumerable.Range(1, listLenght).Select(_ => new Category(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            )
        ).ToList();

    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;
}
