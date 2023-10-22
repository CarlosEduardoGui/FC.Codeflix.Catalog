using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Common;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest : CategoryBaseFixture
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("EndToEnd/Api", "Category - Endpoints")]
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
            .GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
