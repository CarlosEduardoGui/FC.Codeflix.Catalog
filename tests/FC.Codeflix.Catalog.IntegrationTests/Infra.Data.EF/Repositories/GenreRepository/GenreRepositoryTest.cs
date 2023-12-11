using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Insert))]
    public async Task Insert()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(dbContext);

        await genreRepository.InsertAsync(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToList();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
        });
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var genre = await genreRepository.GetByIdAsync(exampleGenre.Id, CancellationToken.None);

        genre.Should().NotBeNull();
        genre!.Name.Should().Be(exampleGenre.Name);
        genre.IsActive.Should().Be(exampleGenre.IsActive);
        genre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genre.Categories.Should().HaveCount(categoriesListExample.Count);
        genre.Categories.ToList().ForEach(categoryId =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    public async Task GetThrowWhenNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGuid = Guid.NewGuid();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var action = async () => await genreRepository.GetByIdAsync(exampleGuid, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleGuid}' not found.");
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var repositoryDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        await genreRepository.DeleteAsync(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = assertsDbContext.Genres.AsNoTracking().FirstOrDefault(x => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();
        var categoriesIdsList = await dbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        categoriesIdsList.Should().HaveCount(0);
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        await genreRepository.UpdateAsync(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToList();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
        });
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    public async Task UpdateRemovingRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        await genreRepository.UpdateAsync(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToList();
        genreCategoriesRelation.Should().HaveCount(0);
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    public async Task UpdateReplacingRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        var updateCategoriesListExample = _fixture.GetExampleCategoriesList(2);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        updateCategoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await genreRepository.UpdateAsync(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToList();
        genreCategoriesRelation.Should().HaveCount(updateCategoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = updateCategoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
        });
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(SeachReturnsItemsAndTotal))]
    public async Task SeachReturnsItemsAndTotal()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchResult.PerPage);
        searchResult.Total.Should().Be(searchResult.Total);
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        }
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    public async Task SearchReturnsRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres();
        var random = new Random();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation =
                _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category =>
                {
                    exampleGenre.AddCategory(category.Id);
                });
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category => new GenresCategories(category.Id, exampleGenre.Id))
                    .ToList();

                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchResult.PerPage);
        searchResult.Total.Should().Be(searchResult.Total);
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpy))]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpy()
    {
        var actDbContext = _fixture.CreateDbContext();
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchResult.PerPage);
        searchResult.Total.Should().Be(searchResult.Total);
        searchResult.Items.Should().HaveCount(0);
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
            int quantityToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres(quantityToGenerate);
        var random = new Random();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation =
                _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category =>
                {
                    exampleGenre.AddCategory(category.Id);
                });
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category => new GenresCategories(category.Id, exampleGenre.Id))
                    .ToList();

                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.ASC);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchResult.PerPage);
        searchResult.Total.Should().Be(searchResult.Total);
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
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
        var exampleGenresList = _fixture.GetExampleListGenresByNames(new List<string>()
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
        var random = new Random();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation =
                _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category =>
                {
                    exampleGenre.AddCategory(category.Id);
                });
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category => new GenresCategories(category.Id, exampleGenre.Id))
                    .ToList();

                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.ASC);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchResult.PerPage);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);
        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
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
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
            exampleGenresList,
            orderby,
            searchOrder
        );
        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchResult.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = searchResult.Items[i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(expectedItem!.Id);
            outPutItem.Name.Should().Be(expectedItem.Name);
            outPutItem.IsActive.Should().Be(expectedItem.IsActive);
            outPutItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
