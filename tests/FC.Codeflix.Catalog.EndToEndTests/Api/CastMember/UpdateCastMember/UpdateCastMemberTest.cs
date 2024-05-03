using FC.Codeflix.Catalog.Api.ApiModels.CastMember;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class UpdateCastMemberTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public UpdateCastMemberTest(CastMemberBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var exampleCastMember = exampleCastMembersList[10];
        var input = new UpdateCastMemberApiInput(newName, newType);

        var (response, output) = await
            _fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>(
                $"/cast_members/{exampleCastMember.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCastMember.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(input.Type);
        var castMemberFromDb = await _fixture
            .Persistence
            .GetByIdAsync(exampleCastMember.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Id.Should().Be(exampleCastMember.Id);
        castMemberFromDb.Name.Should().Be(newName);
        castMemberFromDb.Type.Should().Be(newType);
    }

    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    [Fact(DisplayName = nameof(UpdateWhenNotFound))]
    public async Task UpdateWhenNotFound()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var newGuid = Guid.NewGuid();
        var input = new UpdateCastMemberApiInput(newName, newType);

        var (response, output) = await
            _fixture.ApiClient.Put<ProblemDetails>(
                $"/cast_members/{newGuid}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{newGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    [Fact(DisplayName = nameof(UpdateErrorValidation))]
    public async Task UpdateErrorValidation()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var newName = string.Empty;
        var newType = _fixture.GetRandomCastMemberType();
        var exampleCastMember = exampleCastMembersList[10];
        var input = new UpdateCastMemberApiInput(newName, newType);

        var (response, output) = await
            _fixture.ApiClient.Put<ProblemDetails>(
                $"/cast_members/{exampleCastMember.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output!.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Title.Should().Be("One or more validation errors occurred");
        output.Detail.Should().Be($"Name should not be empty or null.");
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
