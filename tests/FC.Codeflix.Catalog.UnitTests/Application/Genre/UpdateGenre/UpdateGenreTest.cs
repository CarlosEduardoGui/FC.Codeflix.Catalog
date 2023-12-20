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
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
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
            unitOfWorkMock.Object,
            categoryRepository.Object
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
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
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
            _fixture.GetUnitOfWorkMock().Object,
            categoryRepository.Object
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
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
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
            unitOfWorkMock.Object,
            categoryRepository.Object
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
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
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
            unitOfWorkMock.Object,
            categoryRepository.Object
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

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreAddCategoriesIds))]
    public async Task UpdateGenreAddCategoriesIds()
    {
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var genreExample = _fixture.GetExampleGenre();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !genreExample.IsActive;
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(genreExample);
        categoryRepository.Setup(x =>
            x.GetIdsListByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepository.Object
        );
        var input = new UseCases.UpdateGenreInput(
            genreExample.Id,
            newNameExample,
            newIsActive,
            exampleCategoriesIds
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(genreExample.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(genreExample.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(expectedId => output.Categories.Should().Contain(x => x.Id == expectedId));
        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    public async Task UpdateGenreReplacingCategoriesIds()
    {
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var genreExample = _fixture.GetExampleGenre(
            categoriesIds: _fixture.GetRandomIdsList()
        );
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !genreExample.IsActive;
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.Is<Guid>(x => x == genreExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(genreExample);
        categoryRepository.Setup(x =>
            x.GetIdsListByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepository.Object
        );
        var input = new UseCases.UpdateGenreInput(
            genreExample.Id,
            newNameExample,
            newIsActive,
            exampleCategoriesIds
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(genreExample.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(genreExample.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(expectedId => output.Categories.Should().Contain(x => x.Id == expectedId));
        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: _fixture.GetRandomIdsList()
        );
        var exampleNewCategoriesIdsList = _fixture.GetRandomIdsList(10);
        var listReturnedByCategoryRepository =
            exampleNewCategoriesIdsList
                .GetRange(0, exampleNewCategoriesIdsList.Count - 2);

        var IdsNotReturnedByCategoryRepository =
            exampleNewCategoriesIdsList
                .GetRange(exampleNewCategoriesIdsList.Count - 2, 2);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(listReturnedByCategoryRepository);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCases.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
            newIsActive,
            exampleNewCategoriesIdsList
        );

        var action = async ()
            => await useCase.Handle(input, CancellationToken.None);

        var notFoundIdsAsString = String.Join(
            ", ",
            IdsNotReturnedByCategoryRepository
        );
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage(
            $"Related category Id (or Ids) not found: {notFoundIdsAsString}"
        );
    }

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    public async Task UpdateGenreWithoutCategoriesIds()
    {
        var categoryRepository = _fixture.GetCategoryRepositoryMock();
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var genreExample = _fixture.GetExampleGenre(
            categoriesIds: exampleCategoriesIds
        );
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
            unitOfWorkMock.Object,
            categoryRepository.Object
        );
        var input = new UseCases.UpdateGenreInput(
            genreExample.Id,
            newNameExample,
            newIsActive
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(genreExample.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(genreExample.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(expectedId => output.Categories.Should().Contain(x => x.Id == expectedId));
        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<GenreEntity>(x => x.Id == genreExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Use Cases", "UpdateGenre - Use Cases")]
    [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIdsList))]
    public async Task UpdateGenreWithEmptyCategoriesIdsList()
    {
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: exampleCategoriesIdsList
        );
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        var useCase = new UseCases.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCases.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
        newIsActive,
            new List<Guid>()
        );

        var output =
            await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<GenreEntity>(x => x.Id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
