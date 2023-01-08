using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.ListCategory;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

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
        var input = new ListCategoriesInput(
            page: 2,
            perPage: 15,
            search: "search-example",
            sort: "name",
            dir: SearchOrder.ASC
        );
        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: 70
        );
        repositoryMock.Setup(x => x.Search(
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
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreateAt);
        });
        repositoryMock.Verify(x => x.Search(
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
