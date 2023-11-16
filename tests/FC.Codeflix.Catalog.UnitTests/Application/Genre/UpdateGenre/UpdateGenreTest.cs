using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreOk))]
    public async Task UpdateGenreOk()
    {
        var genreExample = _fixture.GetExampleGenre();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !genreExample.IsActive;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(genreExample);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCases.UpdateGenreInput(genreExample.Id, newNameExample, newIsActive);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(genreExample.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(genreExample.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(ThrowWhenIdNotFound))]
    public async Task ThrowWhenIdNotFound()
    {
        var exampleGuid = Guid.NewGuid();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new NotFoundException($"Genre {exampleGuid} not found."));
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            _fixture.GetUnitOfWorkMock().Object
        );
        var input = new UseCases.UpdateGenreInput(exampleGuid, _fixture.GetValidGenreName());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre {exampleGuid} not found.");

        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<GenreEntity>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ThrowWhenNameIsInvalid(string invalidName)
    {
        var genreExample = _fixture.GetExampleGenre();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !genreExample.IsActive;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(genreExample);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCases.UpdateGenreInput(genreExample.Id, invalidName, newIsActive);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage($"Name should not be empty or null.");

        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<GenreEntity>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Theory(DisplayName = nameof(UpdateGenreNameOnly))]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreNameOnly(bool isActive)
    {
        var genreExample = _fixture.GetExampleGenre(isActive: isActive);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !genreExample.IsActive;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(genreExample);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCases.UpdateGenreInput(genreExample.Id, newNameExample);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(genreExample.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(isActive);
        output.CreatedAt.Should().BeSameDateAs(genreExample.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
