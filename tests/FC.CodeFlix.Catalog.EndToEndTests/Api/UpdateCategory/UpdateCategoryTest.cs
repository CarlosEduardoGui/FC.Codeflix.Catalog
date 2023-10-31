using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [Fact(DisplayName = nameof(UpdateCategory))]
    public async Task UpdateCategory()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];
        var input = _fixture.GetExampleInput(exampleCategory.Id);

        var (response, output) = await
            _fixture.ApiClient.Put<CategoryModelOutput>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        var dbCategory = await _fixture
            .Persistence
            .GetByIdAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive);
    }

    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    public async Task UpdateCategoryOnlyName()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];
        var input = new UpdateCategoryInput(exampleCategory.Id, _fixture.GetValidCategoryName());

        var (response, output) = await
            _fixture.ApiClient.Put<CategoryModelOutput>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        var dbCategory = await _fixture
            .Persistence
            .GetByIdAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive);
    }

    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    public async Task UpdateCategoryNameAndDescription()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];
        var input = new UpdateCategoryInput(
            exampleCategory.Id,
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        var (response, output) = await
            _fixture.ApiClient.Put<CategoryModelOutput>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        var dbCategory = await _fixture
            .Persistence
            .GetByIdAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive);
    }

    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var randomGuid = Guid.NewGuid();
        var input = _fixture.GetExampleInput(randomGuid);

        var (response, output) = await
            _fixture.ApiClient.Put<ProblemDetails>(
                $"/categories/{randomGuid}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }
}
