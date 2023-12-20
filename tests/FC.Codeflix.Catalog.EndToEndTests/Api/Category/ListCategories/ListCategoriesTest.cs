using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.ApiModels;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.Date;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest : IDisposable
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture) =>
        _fixture = fixture;

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoryList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);

        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output!.Data.Should().HaveCount(defaultPerPage);
        foreach (var outputItem in output.Data!)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(0);
        output!.Data.Should().HaveCount(0);
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output!.Data.Should().HaveCount(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Data!)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
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
        var exampleCategoryList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var input = new ListCategoriesInput(page, perPage);

        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedQuantityItems);
        foreach (var outputItem in output.Data!)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
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
        await _fixture.Persistence.InsertListAsync(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage, search);

        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output!.Data.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var outputItem in output.Data!)
        {
            var expectedItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Theory(DisplayName = nameof(SearchOrderedDates))]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrderedDates(
        string orderby,
        string order
    )
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var input = new ListCategoriesInput(1, 20, sort: orderby, dir: searchOrder);

        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output!.Data.Should().HaveCount(exampleCategoryList.Count);
        DateTime? lastItemDate = null;

        foreach (var outputItem in output.Data!)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
            if (lastItemDate != null)
            {
                if (order == "asc")
                {
                    Assert.True(outputItem.CreatedAt >= lastItemDate);
                }
                else
                {
                    Assert.True(outputItem.CreatedAt <= lastItemDate);
                }

                lastItemDate = outputItem.CreatedAt;
            }
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderby,
        string order
    )
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var input = new ListCategoriesInput(1, 20, sort: orderby, dir: searchOrder);

        var (response, output) = await
            _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output!.Data.Should().HaveCount(exampleCategoryList.Count);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
           exampleCategoryList,
           orderby,
           searchOrder
       );

        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Data![i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(expectedItem!.Id);
            outPutItem.Name.Should().Be(expectedItem.Name);
            outPutItem.Description.Should().Be(expectedItem.Description);
            outPutItem.IsActive.Should().Be(expectedItem.IsActive);
            outPutItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
        }
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
