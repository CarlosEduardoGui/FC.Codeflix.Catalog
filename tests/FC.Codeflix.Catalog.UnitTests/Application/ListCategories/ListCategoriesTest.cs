using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Application.ListCategory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
            dir: SearchOrder.Asc
        );
        var outputRepositorySearch = new OutputSearch<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            Items: (IReadOnlyList<Category>)categoriesExampleList,
            Total: 70
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput.Page == input.Page,
                && searchInput.PerPage == input.PerPage,
                && searchInput.Search == input.Search,
                && searchInput.OrderBy == input.Sort,
                && searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new ListCategories(repositoryMock.Object);

        var outPut = await userase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.Page.Should().Be(outputRepositorySearch.currentPage);
        outPut.PerPage.Should().Be(outputRepositorySearch.PerPage);
        outPut.Total.Should().Be(outputRepositorySearch.Total);
        outPut.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        outPut.Items.Foreach(outputItem => 
        {
            var repositoryCategory = outputRepositorySearch.Items.Find(x => x.Id == outputItem.Id);
            outPutItem.Should().NotBeNull();
            outPutItem.Name.Should().Be(repositoryCategory.Name);
            outPutItem.Description.Should().Be(repositoryCategory.Description);
            outPutItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outPutItem.CreatedAt.Should().Be(repositoryCategory.CreateAt);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                searchInput.Page == input.Page,
                && searchInput.PerPage == input.PerPage,
                && searchInput.Search == input.Search,
                && searchInput.OrderBy == input.Sort,
                && searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
