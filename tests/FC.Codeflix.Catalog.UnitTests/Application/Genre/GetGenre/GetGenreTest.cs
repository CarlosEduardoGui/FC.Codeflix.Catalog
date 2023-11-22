using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Use Cases", "GetGenre - Use Cases")]
    [Fact(DisplayName = nameof(GetGenreOk))]
    public async Task GetGenreOk()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: _fixture.GetRandomIdsList()
        );
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);
        var useCase = new UseCases.GetGenre(
            genreRepositoryMock.Object
        );
        var input = new UseCases.GetGenreInput(exampleGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleGenre.Categories.Count);
        exampleGenre.Categories.ToList().ForEach(expectedId => output.Categories.Should().Contain(expectedId));
        genreRepositoryMock.Verify(
            x => x.GetByIdAsync(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Trait("Use Cases", "GetGenre - Use Cases")]
    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var randomGenreId = Guid.NewGuid();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(It.Is<Guid>(x => x == randomGenreId), It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Genre '{randomGenreId}' not found."));
        var useCase = new UseCases.GetGenre(
            genreRepositoryMock.Object
        );
        var input = new UseCases.GetGenreInput(randomGenreId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGenreId}' not found.");

        genreRepositoryMock.Verify(
            x => x.GetByIdAsync(
                It.Is<Guid>(x => x == randomGenreId),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }
}
