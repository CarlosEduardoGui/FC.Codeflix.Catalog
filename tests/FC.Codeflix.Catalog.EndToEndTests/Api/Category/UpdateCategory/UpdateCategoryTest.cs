using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest : IDisposable
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
        var input = _fixture.GetExampleApiInput();

        var (response, output) = await
            _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
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
        var input = new UpdateCategoryApiInput(_fixture.GetValidCategoryName());

        var (response, output) = await
            _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive!);
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
        var input = new UpdateCategoryApiInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        var (response, output) = await
            _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive!);
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
        var input = _fixture.GetExampleApiInput();

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

    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstantiateAggregate(
        UpdateCategoryApiInput input,
        string expectedDetail
    )
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];

        var (response, output) = await
            _fixture.ApiClient.Put<ProblemDetails>(
                $"/categories/{exampleCategory.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }
    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
