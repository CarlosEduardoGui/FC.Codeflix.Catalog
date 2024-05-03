using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class CreateCastMemberTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public CreateCastMemberTest(CastMemberBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "CastMember/CreateCastMember - Endpoints")]
    [Fact(DisplayName = nameof(Create))]
    public async Task Create()
    {
        var example = _fixture.GetExampleCastMember();
        var input = new CreateCastMemberInput(example.Name, example.Type);

        var (response, output) =
            await _fixture
            .ApiClient
            .Post<ApiResponse<CastMemberModelOutput>>("/cast_members", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(input.Type);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        var dbCategory = await _fixture
           .Persistence
           .GetByIdAsync(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Type.Should().Be(input.Type);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("EndToEnd/API", "CastMember/CreateCastMember - Endpoints")]
    [Fact(DisplayName = nameof(TryCreateWhenNameIsEmpty))]
    public async Task TryCreateWhenNameIsEmpty()
    {
        var example = _fixture.GetExampleCastMember();
        var input = new CreateCastMemberInput(string.Empty, example.Type);

        var (response, output) =
            await _fixture
            .ApiClient
            .Post<ProblemDetails>("/cast_members", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be("Name should not be empty or null.");
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
