using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Entity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.ListCastMembers;

[Collection(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTest
{
    private readonly ListCastMembersTestFixture _fixture;

    public ListCastMembersTest(ListCastMembersTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "ListCastMembers - Use Cases")]
    [Fact(DisplayName = nameof(List))]
    public async Task List()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMembersListExample = _fixture.GetExampleCastMembersList(3);
        var repositorySearchOutput = new SearchOutput<Entity.CastMember>(
            1,
            10,
            castMembersListExample.AsReadOnly(),
            castMembersListExample.Count
        );
        repositoryMock.Setup(x => x.SearchAsync(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            repositorySearchOutput
        );
        var input = new ListCastMembersInput(
            1,
            10,
            "",
            "",
            SearchOrder.ASC
        );
        var useCase = new UseCase.ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(repositorySearchOutput.CurrentPage);
        output.PerPage.Should().Be(repositorySearchOutput.PerPage);
        output.Total.Should().Be(repositorySearchOutput.Total);
        output.Items.ToList().ForEach(outputItem =>
        {
            var example = castMembersListExample.Find(x => x.Id == outputItem.Id);

            example.Should().NotBeNull();
            outputItem.Name.Should().Be(example!.Name);
            outputItem.Type.Should().Be(example.Type);
        });
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(x =>
                x.Page == input.Page
                && x.PerPage == input.PerPage
                && x.Search == input.Search
                && x.SearchOrder == input.Dir
                && x.OrderBy == input.Sort
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "ListCastMembers - Use Cases")]
    [Fact(DisplayName = nameof(ListEmpty))]
    public async Task ListEmpty()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMemmberListExample = new List<Entity.CastMember>();
        var repositorySearchOutput = new SearchOutput<Entity.CastMember>(
            1,
            10,
            castMemmberListExample,
            0
        );
        repositoryMock.Setup(x => x.SearchAsync(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            repositorySearchOutput
        );
        var input = new ListCastMembersInput(
            1,
            10,
            "",
            "",
            SearchOrder.ASC
        );
        var useCase = new UseCase.ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(repositorySearchOutput.CurrentPage);
        output.PerPage.Should().Be(repositorySearchOutput.PerPage);
        output.Total.Should().Be(repositorySearchOutput.Total);
        output.Items.Should().HaveCount(castMemmberListExample.Count);
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(x =>
                x.Page == input.Page
                && x.PerPage == input.PerPage
                && x.Search == input.Search
                && x.SearchOrder == input.Dir
                && x.OrderBy == input.Sort
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
