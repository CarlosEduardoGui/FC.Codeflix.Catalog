using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Xunit;
using ListGenresUseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre.ListGenres;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest
{
    private readonly ListGenresTestFixture _fixture;

    public ListGenresTest(ListGenresTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [Fact(DisplayName = nameof(ListGenresOk))]
    public async Task ListGenresOk()
    {
        var genresList = _fixture.GetExampleListGenres();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(genresList);
        await arrangeDbContext.SaveChangesAsync();
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(_fixture.CreateDbContext(true))
        );
        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresList.Count);
        output.Items.Should().HaveCount(genresList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var genre = genresList.Find(x => x.Id == outputItem.Id);
            genre.Should().NotBeNull();
            genre!.Name.Should().Be(outputItem.Name);
            genre.IsActive.Should().Be(outputItem.IsActive);
            genre.CreatedAt.Should().Be(outputItem.CreatedAt);
            genre.Categories.Should().BeEquivalentTo(outputItem.Categories);
        });
    }

    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [Fact(DisplayName = nameof(ListGenresReturnsEmptyWhenPersistenceIsEmpty))]
    public async Task ListGenresReturnsEmptyWhenPersistenceIsEmpty()
    {
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(_fixture.CreateDbContext(true))
        );
        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [Fact(DisplayName = nameof(ListGenresWithRelations))]
    public async Task ListGenresWithRelations()
    {
        var genresList = _fixture.GetExampleListGenres();
        var categoryList = _fixture.GetExampleCategoriesList();
        var arrangeDbContext = _fixture.CreateDbContext();
        var random = new Random();
        genresList.ForEach(genre =>
        {
            var relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                var randomCategory = random.Next(0, categoryList.Count - 1);
                var selected = categoryList[randomCategory];
                if (genre.Categories.Contains(selected.Id) is not true)
                    genre.AddCategory(selected.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        genresList.ForEach(genre =>
        {
            genre.Categories.ToList().ForEach(categoryId =>
            {
                genresCategories.Add(new GenresCategories(categoryId, genre.Id));
            });
        });
        await arrangeDbContext.Categories.AddRangeAsync(categoryList);
        await arrangeDbContext.Genres.AddRangeAsync(genresList);
        await arrangeDbContext.GenresCategories.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(_fixture.CreateDbContext(true))
        );
        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresList.Count);
        output.Items.Should().HaveCount(genresList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var genre = genresList.Find(x => x.Id == outputItem.Id);
            genre.Should().NotBeNull();
            outputItem!.Name.Should().Be(genre!.Name);
            outputItem.IsActive.Should().Be(genre.IsActive);
            outputItem.CreatedAt.Should().Be(genre.CreatedAt);
            var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(genre.Categories);
        });
    }
}
