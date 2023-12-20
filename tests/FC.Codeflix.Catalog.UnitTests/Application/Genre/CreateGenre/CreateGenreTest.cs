using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Use Cases", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateGenreOk))]
    public async Task CreateGenreOk()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = _fixture.GetInput();

        var outPut = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<GenreEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        outPut.Should().NotBeNull();
        outPut.Id.Should().NotBeEmpty();
        outPut.Name.Should().Be(input.Name);
        outPut.IsActive.Should().Be(input.IsAtive);
        outPut.Categories.Should().HaveCount(0);
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Use Cases", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
    public async Task CreateWithRelatedCategories()
    {
        var input = _fixture.GetInputWithCategories();
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        categoryRepositoryMock.Setup(x =>
            x.GetIdsListByIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            input.CategoriesIds!
        );

        var useCase = new UseCases.CreateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var outPut = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<GenreEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        outPut.Should().NotBeNull();
        outPut.Id.Should().NotBeEmpty();
        outPut.Name.Should().Be(input.Name);
        outPut.IsActive.Should().Be(input.IsAtive);
        outPut.Categories.Should().HaveCount(input.CategoriesIds!.Count);
        input.CategoriesIds.ForEach(id =>
        {
            outPut.Categories.Should().Contain(x => x.Id == id);
        });
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Use Cases", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
    public void CreateThrowWhenRelatedCategoryNotFound()
    {
        var input = _fixture.GetInputWithCategories();
        var exampleGuid = input.CategoriesIds![^1];
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        categoryRepositoryMock.Setup(x =>
            x.GetIdsListByIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            input.CategoriesIds.FindAll(x => x != exampleGuid)
        );

        var useCase = new UseCases.CreateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related category Id (or Ids) not found: '{exampleGuid}'");
    }

    [Trait("Use Cases", "CreateGenre - Use Cases")]
    [Theory(DisplayName = nameof(CreateThrowWhenNameIsInvalid))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void CreateThrowWhenNameIsInvalid(string invalidName)
    {
        var input = _fixture.GetInput(invalidName);
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage($"'Name' should not be empty or null.");
    }
}
