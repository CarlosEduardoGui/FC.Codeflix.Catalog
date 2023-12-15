using FluentAssertions;
using Xunit;
using CreateGenreUseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre.CreateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateGenre))]
    public async Task CreateGenre()
    {
        var input = _fixture.GetExampleInput();
        var actDbContext = _fixture.CreateDbContext();
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var createGenreUseCase = new CreateGenreUseCase(genreRepository, unitOfWork, categoryRepository);

        var output = await createGenreUseCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsAtive);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(0);
        var assertDbContext = _fixture.CreateDbContext(true);
        var genreDb = await assertDbContext.Genres.FindAsync(output.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(output.Id);
        genreDb.Name.Should().Be(output.Name);
        genreDb.IsActive.Should().Be(output.IsActive);
        genreDb.CreatedAt.Should().Be(output.CreatedAt);
        genreDb.Categories.Should().HaveCount(output.Categories.Count);
    }
}
