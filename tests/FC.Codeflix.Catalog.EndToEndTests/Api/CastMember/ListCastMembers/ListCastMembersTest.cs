using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.ApiModels;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMembers;

[Collection(nameof(CastMemberBaseFixture))]
public class ListCastMembersTest
{
    private readonly CastMemberBaseFixture _fixture;

    public ListCastMembersTest(CastMemberBaseFixture fixture) 
        => _fixture = fixture;

    [Trait("EndToEnd/API", "CastMember/ListCastMember - Endpoints")]
    [Fact(DisplayName = nameof(List))]
    public async Task List()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(10);
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/cast_members");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(exampleCastMembersList.Count);
        output.Data.Should().HaveCount(exampleCastMembersList.Count);
        output.Data!.ForEach(outputItem =>
        {
            var item = exampleCastMembersList.Find(x => x.Id == outputItem.Id);

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(outputItem);
        });
    }
}
