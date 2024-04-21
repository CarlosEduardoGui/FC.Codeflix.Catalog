using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private readonly CastMemberRepositoryTestFixture _fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Insert))]
    public async Task Insert()
    {
        var castMemberExample = _fixture.GetExampleCastMember();
        var context = _fixture.CreateDbContext();
        var repository = new Repository.CastMemberRepository(context);

        await repository.InsertAsync(castMemberExample, CancellationToken.None);
        await context.SaveChangesAsync();

        var assertionContext = _fixture.CreateDbContext(true);
        var castMemberFromDb = assertionContext
            .CastMembers
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == castMemberExample.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(castMemberExample.Name);
        castMemberFromDb.Type.Should().Be(castMemberExample.Type);
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));

        var itemFromRepository = await repository.GetByIdAsync(
            castMemberExample.Id,
            CancellationToken.None
        );

        itemFromRepository.Should().NotBeNull();
        itemFromRepository!.Name.Should().Be(castMemberExample.Name);
        itemFromRepository.Type.Should().Be(castMemberExample.Type);
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    public async Task GetThrowsWhenNotFound()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberRandomGuid = Guid.NewGuid();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));

        var action = async () => await repository.GetByIdAsync(
            castMemberRandomGuid,
            CancellationToken.None
        );

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"CastMember '{castMemberRandomGuid}' not found.");
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);

        await repository.DeleteAsync(
            castMemberExample,
            CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertionContext = _fixture.CreateDbContext(true);
        var castMemberFromDb = assertionContext
            .CastMembers
            .AsNoTracking()
            .ToList();
        castMemberFromDb.Should().HaveCount(castMemberExampleList.Count - 1);
        castMemberFromDb.Should().NotContain(castMemberExample);
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);
        castMemberExample.Update(newName, newType);

        await repository.UpdateAsync(
            castMemberExample,
            CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertionContext = _fixture.CreateDbContext(true);
        var castMemberFromDb = await assertionContext
            .CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(newName);
        castMemberFromDb.Type.Should().Be(newType);
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Search))]
    public async Task Search()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
        var searchInput = new SearchInput(
            1,
            10,
            "",
            "",
            SearchOrder.ASC
        );

        var searchResult = await repository.SearchAsync(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(10);
        searchResult.Total.Should().Be(10);
        searchResult.Items.ToList().ForEach(searchItem =>
        {
            var example = castMemberExampleList.Find(x => x.Id == searchItem.Id);

            example.Should().NotBeNull();
            searchItem.Name.Should().Be(example!.Name);
            searchItem.Type.Should().Be(example.Type);
        });
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(SearchEmpty))]
    public async Task SearchEmpty()
    {
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext());
        var searchInput = new SearchInput(
            1,
            10,
            "",
            "",
            SearchOrder.ASC
        );

        var searchResult = await repository.SearchAsync(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(10);
        searchResult.Total.Should().Be(0);
        searchResult.Items.Should().BeEmpty();
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Theory(DisplayName = nameof(SearchPaginated))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchPaginated(
            int quantityToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(quantityToGenerate);
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
        var searchInput = new SearchInput(
            page,
            perPage,
            "",
            "",
            SearchOrder.ASC
        );

        var searchResult = await repository.SearchAsync(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(quantityToGenerate);
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        searchResult.Items.ToList().ForEach(searchItem =>
        {
            var example = castMemberExampleList.Find(x => x.Id == searchItem.Id);

            example.Should().NotBeNull();
            searchItem.Name.Should().Be(example!.Name);
            searchItem.Type.Should().Be(example.Type);
        });
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
        var castMemberExampleList = _fixture.GetExampleCategoriesListWithNames(new List<string>()
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
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
        var searchInput = new SearchInput(
            page,
            perPage,
            search,
            "",
            SearchOrder.ASC
        );

        var searchResult = await repository.SearchAsync(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);
        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
        searchResult.Items.ToList().ForEach(searchItem =>
        {
            var example = castMemberExampleList.Find(x => x.Id == searchItem.Id);

            example.Should().NotBeNull();
            searchItem.Name.Should().Be(example!.Name);
            searchItem.Type.Should().Be(example.Type);
        });
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
        var castMemberExampleList = _fixture.GetExampleCastMembersList();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);
        await arrangeContext.SaveChangesAsync(CancellationToken.None);

        var output = await repository.SearchAsync(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(
            castMemberExampleList,
            orderby,
            searchOrder
        );
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(castMemberExampleList.Count);
        output.Items.Should().HaveCount(castMemberExampleList.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(expectedItem!.Id);
            outPutItem.Name.Should().Be(expectedItem.Name);
            outPutItem.Type.Should().Be(expectedItem.Type);
        }

    }
}
