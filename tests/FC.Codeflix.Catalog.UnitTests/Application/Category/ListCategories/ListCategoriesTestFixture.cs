using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }
public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{
    public List<CategoryEntity> GetExampleCategoriesList(int length = 10)
    {
        var list = new List<CategoryEntity>();
        for (int i = 0; i < length; i++)
            list.Add(GetExampleCategory());

        return list;
    }

    public ListCategoriesInput GetExampleInput()
    {
        var random = new Random();
        return new ListCategoriesInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.ASC : SearchOrder.DESC
        );
    }
}
