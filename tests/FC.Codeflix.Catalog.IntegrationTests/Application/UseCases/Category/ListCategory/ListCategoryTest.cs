using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using ListCategoriesUseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories.ListCategories;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategory;

[Collection(nameof(ListCategoryTestFixture))]
public class ListCategoryTest
{
    private readonly ListCategoryTestFixture _fixture;

    public ListCategoryTest(ListCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Integration/Application", "ListCategory - Use Case")]
    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    public async Task SearchReturnsListAndTotal()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(1, 20);
        await dbContext.SaveChangesAsync();
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        foreach (CategoryModelOutput outPutItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category =>
                category.Id == outPutItem.Id
            );
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(exampleItem!.Id);
            outPutItem.Name.Should().Be(exampleItem.Name);
            outPutItem.Description.Should().Be(exampleItem.Description);
            outPutItem.IsActive.Should().Be(exampleItem.IsActive);
            outPutItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Trait("Integration/Application", "ListCategory - Use Case")]
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var dbContext = _fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(1, 20);
        await dbContext.SaveChangesAsync();
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Trait("Integration/Application", "ListCategory - Use Case")]
    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(page, perPage, "", "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        foreach (CategoryModelOutput outPutItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category =>
                category.Id == outPutItem.Id
            );
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(exampleItem!.Id);
            outPutItem.Name.Should().Be(exampleItem.Name);
            outPutItem.Description.Should().Be(exampleItem.Description);
            outPutItem.IsActive.Should().Be(exampleItem.IsActive);
            outPutItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Trait("Integration/Application", "CategoryRepository - Repositories")]
    [Theory(DisplayName = nameof(SearchByText))]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Sci-fi", 1, 5, 5, 6)]
    [InlineData("Sci-fi", 1, 2, 2, 6)]
    [InlineData("Sci-fi", 2, 5, 1, 6)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Future",
            "Sci-fi",
            "Sci-fi Robots",
            "Sci-fi StarWars",
            "Sci-fi StarTrek"
        });
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(page, perPage, search, "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (CategoryModelOutput outPutItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category =>
                category.Id == outPutItem.Id
            );
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(exampleItem!.Id);
            outPutItem.Name.Should().Be(exampleItem.Name);
            outPutItem.Description.Should().Be(exampleItem.Description);
            outPutItem.IsActive.Should().Be(exampleItem.IsActive);
            outPutItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Trait("Integration/Application", "CategoryRepository - Repositories")]
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderby,
        string order
    )
    {
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);
        await dbContext.SaveChangesAsync();

        var output = await categoryRepository.SearchAsync(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
            exampleCategoriesList,
            orderby,
            searchOrder
        );
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(expectedItem!.Id);
            outPutItem.Name.Should().Be(expectedItem.Name);
            outPutItem.Description.Should().Be(expectedItem.Description);
            outPutItem.IsActive.Should().Be(expectedItem.IsActive);
            outPutItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
