using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Use Cases", "ListCategories - Use Cases")]
    public async Task List()
    {
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<CategoryEntity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: new Random().Next(50, 200)
        );
        repositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new UseCase.ListCategories(repositoryMock.Object);

        var outPut = await userase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.CurrentPage.Should().Be(outputRepositorySearch.CurrentPage);
        outPut.PerPage.Should().Be(outputRepositorySearch.PerPage);
        outPut.Total.Should().Be(outputRepositorySearch.Total);
        outPut.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<CategoryModelOutput>)outPut.Items).ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory.Name);
            outputItem.Description.Should().Be(repositoryCategory.Description);
            outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
        });
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }


    [Fact(DisplayName = nameof(ListOkWhenEmpty))]
    [Trait("Use Cases", "ListCategories - Use Cases")]
    public async Task ListOkWhenEmpty()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<CategoryEntity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<CategoryEntity>().AsReadOnly(),
            total: 0
        );
        repositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new UseCase.ListCategories(repositoryMock.Object);

        var outPut = await userase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.CurrentPage.Should().Be(outputRepositorySearch.CurrentPage);
        outPut.PerPage.Should().Be(outputRepositorySearch.PerPage);
        outPut.Total.Should().Be(0);
        outPut.Items.Should().HaveCount(0);
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory(DisplayName = nameof(ListWithoutAllParameters))]
    [Trait("Use Cases", "ListCategories - Use Cases")]
    [MemberData(
        nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameter),
        parameters: 14,
        MemberType = typeof(ListCategoriesTestDataGenerator)
    )]
    public async Task ListWithoutAllParameters(ListCategoriesInput input)
    {
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var outputRepositorySearch = new SearchOutput<CategoryEntity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: new Random().Next(50, 200)
        );
        repositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new UseCase.ListCategories(repositoryMock.Object);

        var outPut = await userase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.CurrentPage.Should().Be(outputRepositorySearch.CurrentPage);
        outPut.PerPage.Should().Be(outputRepositorySearch.PerPage);
        outPut.Total.Should().Be(outputRepositorySearch.Total);
        outPut.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<CategoryModelOutput>)outPut.Items).ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory!.Name);
            outputItem.Description.Should().Be(repositoryCategory.Description);
            outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
        });
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
