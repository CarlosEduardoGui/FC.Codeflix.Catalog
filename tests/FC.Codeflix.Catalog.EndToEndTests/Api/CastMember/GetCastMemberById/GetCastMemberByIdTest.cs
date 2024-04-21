using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.Date;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.GetCastMemberById;

[Collection(nameof(CastMemberBaseFixture))]
public class GetCastMemberByIdTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public GetCastMemberByIdTest(CastMemberBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "CastMember/GetCastMember - Endpoints")]
    [Fact(DisplayName = nameof(GetCastMember))]
    public async Task GetCastMember()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var exampleCastMember = exampleCastMembersList[10];

        var (response, output) = await
            _fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>(
                $"/cast_members/{exampleCastMember.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCastMember.Id);
        output.Data.Name.Should().Be(exampleCastMember.Name);
        output.Data.CreatedAt.TrimMillisseconds().Should().Be(exampleCastMember.CreatedAt.TrimMillisseconds());
    }

    [Trait("EndToEnd/API", "CastMember/GetCastMember - Endpoints")]
    [Fact(DisplayName = nameof(GetCastMemberWhenNotFound))]
    public async Task GetCastMemberWhenNotFound()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await
            _fixture.ApiClient.Get<ProblemDetails>(
                $"/cast_members/{randomGuid}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

    public void Dispose() =>
        _fixture.CleanPersistence();
}
