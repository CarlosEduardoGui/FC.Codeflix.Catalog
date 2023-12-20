using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.ListGenre;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest
{
    private readonly ListGenreTestFixture _fixture;

    public ListGenreTest(ListGenreTestFixture fixture) =>
        _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Use Cases", "ListGenre - Use Cases")]
    public async Task List()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<GenreEntity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: genresExampleList,
            total: new Random().Next(50, 200)
        );
        genreRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new ListGenres(genreRepositoryMock.Object, categoryRepository.Object);

        ListGenresOutPut outPut = await userase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.CurrentPage.Should().Be(outputRepositorySearch.CurrentPage);
        outPut.PerPage.Should().Be(outputRepositorySearch.PerPage);
        outPut.Total.Should().Be(outputRepositorySearch.Total);
        outPut.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<GenreModelOutPut>)outPut.Items).ForEach(outputItem =>
        {
            var repositoryGenre = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryGenre!.Name);
            outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
            outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);
            foreach (var expectedId in repositoryGenre.Categories)
                outputItem.Categories.Should().Contain(x => x.Id == expectedId);
        });
        genreRepositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        var expectedIds = genresExampleList
            .SelectMany(genre => genre.Categories)
            .Distinct()
            .ToList();
        categoryRepository.Verify(x => x.GetListByIdsAsync(
            It.Is<List<Guid>>(parameterList =>
                expectedIds.All(id => parameterList.Contains(id)
                && expectedIds.Count == parameterList.Count
            )),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ListEmpty))]
    [Trait("Use Cases", "ListGenre - Use Cases")]
    public async Task ListEmpty()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<GenreEntity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<GenreEntity>(),
            total: new Random().Next(50, 200)
        );
        genreRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new ListGenres(genreRepositoryMock.Object, categoryRepository.Object);

        ListGenresOutPut outPut = await userase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.CurrentPage.Should().Be(outputRepositorySearch.CurrentPage);
        outPut.PerPage.Should().Be(outputRepositorySearch.PerPage);
        outPut.Total.Should().Be(outputRepositorySearch.Total);
        outPut.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        genreRepositoryMock.Verify(x => x.SearchAsync(
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

    [Fact(DisplayName = nameof(ListUsingDefaultInputValues))]
    [Trait("Use Cases", "ListGenre - Use Cases")]
    public async Task ListUsingDefaultInputValues()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var outputRepositorySearch = new SearchOutput<GenreEntity>(
            currentPage: 1,
            perPage: 15,
            items: new List<GenreEntity>(),
            total: 0
        );
        genreRepositoryMock.Setup(x => x.SearchAsync(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var userase = new ListGenres(genreRepositoryMock.Object, categoryRepository.Object);

        ListGenresOutPut outPut = await userase.Handle(new UseCases.ListGenresInput(), CancellationToken.None);

        genreRepositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == 1
                && searchInput.PerPage == 15
                && searchInput.Search == ""
                && searchInput.OrderBy == ""
                && searchInput.SearchOrder == SearchOrder.ASC
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
