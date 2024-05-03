using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class DeleteCastMemberTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public DeleteCastMemberTest(CastMemberBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "CastMember/DeleteCastMember - Endpoints")]
    [Fact(DisplayName = nameof(DeleteCastMember))]
    public async Task DeleteCastMember()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var exampleCastMember = exampleCastMembersList[10];

        var (response, output) = await
            _fixture.ApiClient.Delete<ApiResponse<CastMemberModelOutput>>(
                $"/cast_members/{exampleCastMember.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var castMemberFomDb = await _fixture.Persistence.GetByIdAsync(exampleCastMember.Id);
        castMemberFomDb.Should().BeNull();
    }

    [Trait("EndToEnd/API", "CastMember/DeleteCastMember - Endpoints")]
    [Fact(DisplayName = nameof(NotDeleteCastMemberWhenNotFound))]
    public async Task NotDeleteCastMemberWhenNotFound()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var exampleCastMember = Guid.NewGuid();

        var (response, output) = await
            _fixture.ApiClient.Delete<ProblemDetails>(
                $"/cast_members/{exampleCastMember}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{exampleCastMember}' not found.");
        output.Type.Should().Be("NotFound");
        var castMemberFomDb = await _fixture.Persistence.GetByIdAsync(exampleCastMember);
        castMemberFomDb.Should().BeNull();
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
