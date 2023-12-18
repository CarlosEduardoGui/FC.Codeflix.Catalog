using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
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
}
