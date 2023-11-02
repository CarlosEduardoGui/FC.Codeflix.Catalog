using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest : IDisposable
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("EndToEnd/Api", "Category/Create - Endpoints")]
    [Fact(DisplayName = nameof(CreateCategoryOk))]
    public async Task CreateCategoryOk()
    {
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient.Post<CategoryModelOutput>(
            "/categories",
            input
        );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        var dbCategory = await _fixture
            .Persistence
            .GetByIdAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }


    [Trait("EndToEnd/Api", "Category/Create - Endpoints")]
    [Theory(DisplayName = nameof(ErrorWhenCantInstanciateAggregate))]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstanciateAggregate(
        CreateCategoryInput input,
        string expectedDetail
    )
    {
        var (response, output) = await _fixture.ApiClient.Post<ProblemDetails>(
            "/categories",
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
