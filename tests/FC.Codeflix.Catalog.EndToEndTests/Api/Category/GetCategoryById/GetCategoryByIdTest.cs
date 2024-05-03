using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.Date;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategoryById;

[Collection(nameof(CategoryBaseFixture))]
public class GetCategoryByIdTest : IDisposable
{
    private readonly CategoryBaseFixture _fixture;

    public GetCategoryByIdTest(CategoryBaseFixture fixture) =>
        _fixture = fixture;

    [Trait("EndToEnd/API", "Category/GetCategory - Endpoints")]
    [Fact(DisplayName = nameof(GetCategory))]
    public async Task GetCategory()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];

        var (response, output) = await
            _fixture.ApiClient.Get<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(exampleCategory.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive);
        output.Data.CreatedAt.TrimMillisseconds().Should().Be(exampleCategory.CreatedAt.TrimMillisseconds());
    }

    [Trait("EndToEnd/API", "Category/GetCategory - Endpoints")]
    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await
            _fixture.ApiClient.Get<ProblemDetails>(
                $"/categories/{randomGuid}"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
