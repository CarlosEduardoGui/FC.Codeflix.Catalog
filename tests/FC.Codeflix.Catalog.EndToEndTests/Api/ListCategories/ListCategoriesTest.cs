using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.ListCategories;

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
            _fixture.ApiClient.Get<ListCategoriesOutput>(
                $"/categories"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output.CurrentPage.Should().Be(1);
        output.PerPage.Should().Be(defaultPerPage);
        output!.Items.Should().HaveCount(defaultPerPage);
        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await
            _fixture.ApiClient.Get<ListCategoriesOutput>(
                $"/categories"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(0);
        output!.Items.Should().HaveCount(0);
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        var (response, output) = await
            _fixture.ApiClient.Get<ListCategoriesOutput>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output!.Items.Should().HaveCount(input.PerPage);
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
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
            _fixture.ApiClient.Get<ListCategoriesOutput>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output!.Items.Should().HaveCount(expectedQuantityItems);
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoryList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
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
            _fixture.ApiClient.Get<ListCategoriesOutput>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(expectedQuantityTotalItems);
        output!.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);

            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem!.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
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
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var input = new ListCategoriesInput(1, 20, sort: orderby, dir: searchOrder);

        var (response, output) = await
            _fixture.ApiClient.Get<ListCategoriesOutput>(
                $"/categories", input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output!.Items.Should().HaveCount(exampleCategoryList.Count);
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
           exampleCategoryList,
           orderby,
           searchOrder
       );

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

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
