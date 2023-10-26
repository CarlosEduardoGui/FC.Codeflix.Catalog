using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.GetCategoryById;

[Collection(nameof(GetCategoryByIdTestFixture))]
public class GetCategoryByIdTest
{
    private readonly GetCategoryByIdTestFixture _fixture;

    public GetCategoryByIdTest(GetCategoryByIdTestFixture fixture) =>
        _fixture = fixture;

    [Trait("EndToEnd/API", "Category/GetCategory - Endpoints")]
    [Fact(DisplayName = nameof(GetCategory))]
    public async Task GetCategory()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];

        var (response, output) = await
            _fixture.ApiClient.Get<CategoryModelOutput>(
                $"/categories/{exampleCategory.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }
}
