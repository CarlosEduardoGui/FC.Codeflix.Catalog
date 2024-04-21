using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;

[Collection(nameof(ListCastMemberTestFixture))]
public class ListCastMemberTest
{
    private readonly ListCastMemberTestFixture _fixture;

    public ListCastMemberTest(ListCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "ListCastMember - Use Case")]
    [Fact(DisplayName = nameof(List))]
    public async Task List()
    {
        var examples = _fixture.GetExampleCastMembersList();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = _fixture.CastMemberRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.ASC);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(examples.Count);
        output.Total.Should().Be(examples.Count);
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            exampleItem.Should().BeEquivalentTo(outputItem);
        });
    }

    [Trait("Integration/Application", "ListCastMember - Use Case")]
    [Fact(DisplayName = nameof(ListWhenEmpty))]
    public async Task ListWhenEmpty()
    {
        var repository = _fixture.CastMemberRepository(_fixture.CreateDbContext());
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.ASC);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(0);
        output.Total.Should().Be(0);
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
    }

    [Trait("Integration/Application", "ListCastMember - Use Case")]
    [Theory(DisplayName = nameof(ListPagineted))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPagineted(
            int quantityCastMemberToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(quantityCastMemberToGenerate);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = _fixture.CastMemberRepository(dbContext);
        var searchInput = new ListCastMembersInput(page, perPage, "", "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();
        var useCase = new UseCase.ListCastMembers(castMemberRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCastMembersList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        foreach (CastMemberModelOutput outPutItem in output.Items)
        {
            var exampleItem = exampleCastMembersList.Find(castMember =>
                castMember.Id == outPutItem.Id
            );
            outPutItem.Should().NotBeNull();
            outPutItem.Should().BeEquivalentTo(exampleItem);
        }
    }

    [Trait("Integration/Application", "ListCastMember - Use Case")]
    [Theory(DisplayName = nameof(SearchByText))]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Sci-fi", 1, 5, 5, 6)]
    [InlineData("Sci-fi", 1, 2, 2, 6)]
    [InlineData("Sci-fi", 2, 5, 1, 6)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMemberList = _fixture.GetExampleCastMembersListWithNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Future",
            "Sci-fi",
            "Sci-fi Robots",
            "Sci-fi StarWars",
            "Sci-fi StarTrek"
        });
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = _fixture.CastMemberRepository(dbContext);
        var searchInput = new ListCastMembersInput(page, perPage, search, "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();
        var useCase = new UseCase.ListCastMembers(castMemberRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (CastMemberModelOutput outPutItem in output.Items)
        {
            var exampleItem = exampleCastMemberList.Find(castMember =>
                castMember.Id == outPutItem.Id
            );
            outPutItem.Should().NotBeNull();
            outPutItem.Should().BeEquivalentTo(exampleItem);
        }
    }

    [Trait("Integration/Application", "ListCastMember - Use Case")]
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderby,
        string order
    )
    {
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMemberList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = _fixture.CastMemberRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);
        await dbContext.SaveChangesAsync();

        var output = await castMemberRepository.SearchAsync(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(
            exampleCastMemberList,
            orderby,
            searchOrder
        );
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCastMemberList.Count);
        output.Items.Should().HaveCount(exampleCastMemberList.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem.Should().BeEquivalentTo(expectedItem);
        }
    }
}
