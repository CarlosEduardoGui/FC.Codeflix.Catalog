using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FluentAssertions;
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

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
