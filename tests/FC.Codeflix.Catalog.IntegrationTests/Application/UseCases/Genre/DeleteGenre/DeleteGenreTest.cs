using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DeleteGenreUseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre.DeleteGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    [Fact(DisplayName = nameof(DeleteGenreOk))]
    public async Task DeleteGenreOk()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var targetGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = new GenreRepository(actDbContext);
        var useCase = new DeleteGenreUseCase(genreRepository, unitOfWork);
        var input = new DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
    }

    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
    public async Task DeleteGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = new GenreRepository(actDbContext);
        var useCase = new DeleteGenreUseCase(genreRepository, unitOfWork);
        var invalidGenreId = Guid.NewGuid();
        var input = new DeleteGenreInput(invalidGenreId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().
            ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{invalidGenreId}' not found.");
    }

    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    public async Task DeleteGenreWithRelations()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var targetGenre = genresExampleList[5];
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            exampleCategoriesList.Select(category => new GenresCategories(category.Id, targetGenre.Id))
        );
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var genreRepository = new GenreRepository(actDbContext);
        var useCase = new DeleteGenreUseCase(genreRepository, unitOfWork);
        var input = new DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
        var relationsFromDb = assertDbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == targetGenre.Id).ToList();
        relationsFromDb.Should().HaveCount(0);
    }
}
