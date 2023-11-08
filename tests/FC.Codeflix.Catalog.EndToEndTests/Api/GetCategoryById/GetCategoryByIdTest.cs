using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.Date;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.GetCategoryById;

[Collection(nameof(GetCategoryByIdTestFixture))]
public class GetCategoryByIdTest : IDisposable
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
        output.CreatedAt.TrimMillisseconds().Should().Be(exampleCategory.CreatedAt.TrimMillisseconds());
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
