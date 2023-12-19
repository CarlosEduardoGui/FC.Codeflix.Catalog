using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UpdateGenreUseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre.UpdateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreOk))]
    public async Task UpdateGenreOk()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenres = _fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        await dbContext.Genres.AddRangeAsync(exampleGenres);
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var useCase = new UpdateGenreUseCase(genreRepository, unitOfWork, categoryRepository);
        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        output.Categories.Should().HaveCount(0);
        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(input.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive.Value);
        genreFromDb.Categories.Should().HaveCount(0);
    }

    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
    public async Task UpdateGenreWithCategoriesRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenres = _fixture.GetExampleListGenres();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategoriesList.GetRange(0, 5);
        var newRelatedCategories = exampleCategoriesList.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.Genres.AddRangeAsync(exampleGenres);
        await dbContext.GenresCategories.AddRangeAsync(relations);
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var useCase = new UpdateGenreUseCase(genreRepository, unitOfWork, categoryRepository);
        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            newRelatedCategories.Select(x => x.Id).ToList()
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        var relatedCategorydsFromOutput =
            output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
        relatedCategorydsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(input.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive.Value);
        var relatedCategoryIdsFromDb = await assertDbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == input.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryIdNotExists))]
    public async Task UpdateGenreThrowsWhenCategoryIdNotExists()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenres = _fixture.GetExampleListGenres();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategoriesList.GetRange(0, 5);
        var newRelatedCategories = exampleCategoriesList.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.Genres.AddRangeAsync(exampleGenres);
        await dbContext.GenresCategories.AddRangeAsync(relations);
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var useCase = new UpdateGenreUseCase(genreRepository, unitOfWork, categoryRepository);
        var categoryIdsToRelate = newRelatedCategories.Select(x => x.Id).ToList();
        var invalidCategoryId = Guid.NewGuid();
        categoryIdsToRelate.Add(invalidCategoryId);
        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            categoryIdsToRelate
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
             .ThrowExactlyAsync<RelatedAggregateException>()
             .WithMessage($"Related category Id (or Ids) not found: {invalidCategoryId}");
    }

    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenNotFound))]
    public async Task UpdateGenreThrowsWhenNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenres = _fixture.GetExampleListGenres();
        await dbContext.Genres.AddRangeAsync(exampleGenres);
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var useCase = new UpdateGenreUseCase(genreRepository, unitOfWork, categoryRepository);
        var invalidGenreId = Guid.NewGuid();
        var input = new UpdateGenreInput(
            invalidGenreId,
            _fixture.GetValidGenreName()
        );

        var output = async () => await useCase.Handle(input, CancellationToken.None);

        await output.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{invalidGenreId}' not found.");
    }

    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
    public async Task UpdateGenreWithoutNewCategoriesRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenres = _fixture.GetExampleListGenres();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategoriesList.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.Genres.AddRangeAsync(exampleGenres);
        await dbContext.GenresCategories.AddRangeAsync(relations);
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var useCase = new UpdateGenreUseCase(genreRepository, unitOfWork, categoryRepository);
        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        var relatedCategoriesIdsExpected = relatedCategories.Select(x => x.Id).ToList();
        var relatedCategorydsFromOutput =
            output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
        relatedCategorydsFromOutput.Should().BeEquivalentTo(relatedCategoriesIdsExpected);
        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(input.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive.Value);
        var relatedCategoryIdsFromDb = await assertDbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == input.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(relatedCategoriesIdsExpected);
    }
}
