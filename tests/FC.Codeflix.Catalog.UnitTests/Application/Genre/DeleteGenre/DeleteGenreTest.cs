using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Use Cases", "DeleteGenre - Use Cases")]
    [Fact(DisplayName = nameof(DeleteGenreOk))]
    public async Task DeleteGenreOk()
    {
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var genreExample = _fixture.GetExampleGenre();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(genreExample);
        genreRepositoryMock.Setup(x =>
            x.DeleteAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        );
        var useCase = new UseCases.DeleteGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCases.DeleteGenreInput(genreExample.Id);

        await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(
            x => x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        genreRepositoryMock.Verify(
            x => x.DeleteAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Use Cases", "DeleteGenre - Use Cases")]
    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var randomGenreId = Guid.NewGuid();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(It.Is<Guid>(x => x == randomGenreId), It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Genre '{randomGenreId}' not found."));
        var useCase = new UseCases.DeleteGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCases.DeleteGenreInput(randomGenreId);

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
