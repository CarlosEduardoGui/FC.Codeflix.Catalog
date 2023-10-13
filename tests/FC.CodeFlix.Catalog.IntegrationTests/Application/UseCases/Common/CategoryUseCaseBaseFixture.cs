using FC.CodeFlix.Catalog.IntegrationTests.Base;
using CategoryEntity = FC.CodeFlix.Catalog.Domain.Entity.Category;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Common;
public class CategoryUseCaseBaseFixture : BaseFixture
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

    public CategoryEntity GetExampleCategory() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            );
    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;

    public List<CategoryEntity> GetExampleCategoriesList(int lengh = 10) =>
        Enumerable.Range(1, lengh)
        .Select(_ => GetExampleCategory()).ToList();
}
