using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using GetGenreUseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre.GetGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "GetGenre - Use Cases")]
    [Fact(DisplayName = nameof(GetGenre))]
    public async Task GetGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new GetGenreUseCase(genreRepository);
        var input = new GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }

    [Trait("Integration/Application", "GetGenre - Use Cases")]
    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    public async Task GetGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var randomGuid = Guid.NewGuid();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new GetGenreUseCase(genreRepository);
        var input = new GetGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }
}
