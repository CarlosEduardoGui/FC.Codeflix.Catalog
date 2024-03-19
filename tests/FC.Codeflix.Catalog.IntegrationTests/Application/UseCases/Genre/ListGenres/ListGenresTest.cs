using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
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
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(actDbContext),
            _fixture.CreateCategoryRepository(actDbContext)
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
        var actDbContext = _fixture.CreateDbContext();
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(actDbContext),
            _fixture.CreateCategoryRepository(actDbContext)
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
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(actDbContext),
            _fixture.CreateCategoryRepository(actDbContext)
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
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = categoryList.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [Theory(DisplayName = nameof(ListGenresPaginated))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListGenresPaginated(
            int quantityToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        var genresList = _fixture.GetExampleListGenres(quantityToGenerate);
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
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(actDbContext),
            _fixture.CreateCategoryRepository(actDbContext)
        );
        var input = new ListGenresInput(page, perPage);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            var genre = genresList.Find(x => x.Id == outputItem.Id);
            genre.Should().NotBeNull();
            outputItem!.Name.Should().Be(genre!.Name);
            outputItem.IsActive.Should().Be(genre.IsActive);
            outputItem.CreatedAt.Should().Be(genre.CreatedAt);
            var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(genre.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = categoryList.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Trait("Integration/Application", "ListGenres - Use Cases")]
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
        var genresList = _fixture.GetExampleListGenresByNames(new List<string>()
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
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(actDbContext),
            _fixture.CreateCategoryRepository(actDbContext)
        );
        var input = new ListGenresInput(page, perPage, search);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.ToList().ForEach(outputItem =>
        {
            var genre = genresList.Find(x => x.Id == outputItem.Id);
            outputItem.Name.Should().Contain(search);
            genre.Should().NotBeNull();
            outputItem!.Name.Should().Be(genre!.Name);
            outputItem.IsActive.Should().Be(genre.IsActive);
            outputItem.CreatedAt.Should().Be(genre.CreatedAt);
            var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(genre.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = categoryList.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderby, string order)
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
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new ListGenresUseCase(
            _fixture.GenreRepository(actDbContext),
            _fixture.CreateCategoryRepository(actDbContext)
        );
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var input = new ListGenresInput(1, 20, "", orderby, searchOrder);

        var output = await useCase.Handle(input, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
            genresList,
            orderby,
            searchOrder
        );
        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresList.Count);
        output.Items.Should().HaveCount(genresList.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outPutItem!.Name.Should().Be(expectedItem!.Name);
            outPutItem.IsActive.Should().Be(expectedItem.IsActive);
            outPutItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            var outputItemCategoryIds = outPutItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(expectedItem.Categories);
            outPutItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = categoryList.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        }
    }
}
