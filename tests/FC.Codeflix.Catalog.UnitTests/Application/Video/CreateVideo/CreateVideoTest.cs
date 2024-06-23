using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Xunit;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoOk))]
    public async Task CreateVideoOk()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Theory(DisplayName = nameof(CreateVideoThrowsIfInvalidInput))]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsIfInvalidInput(
        CreateVideoInput input,
        string expectedValidationError
    )
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        var expectationAssertion = await action
            .Should()
            .ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("There are validation errors.");
        expectationAssertion
            .Which
            .Errors!.ToList()[0]
            .Message
            .Should().Be(expectedValidationError);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    public async Task CreateVideoWithCategoriesIds()
    {
        var catetoryRepositoryMock = _fixture.GetCategoryRepository();
        var categoriesIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        catetoryRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoriesIds);
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            catetoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(categoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
                && video.Categories.All(categoryId => categoriesIds.Contains(categoryId))
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        catetoryRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CategoriesIds.Should().BeEquivalentTo(categoriesIds);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoThrowsWhenCategoryIdDoesnExist))]
    public async Task CreateVideoThrowsWhenCategoryIdDoesnExist()
    {
        var categoriesIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        var removedCategoryId = categoriesIds[2];
        var categoryRepositoryMock = _fixture.GetCategoryRepository();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object, 
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(categoriesIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related category Id (or Ids) not found: {removedCategoryId}.");
        videoRepositoryMock.Verify(x => x.InsertAsync(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        categoryRepositoryMock.VerifyAll();
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    public async Task CreateVideoWithGenresIds()
    {
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var genresIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        genreRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(genresIds);
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(genresIds: genresIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
                && video.Genres.All(genreId => genresIds.Contains(genreId))
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        genreRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CategoriesIds.Should().BeNullOrEmpty();
        output.GenresIds.Should().BeEquivalentTo(genresIds);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenGenreIdInvalid))]
    public async Task ThrowsWhenGenreIdInvalid()
    {
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var genresIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        var removedId = genresIds[2];
        genreRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(genresIds.FindAll(id => id != removedId));
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(genresIds: genresIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related genre Id (or Ids) not found: {removedId}.");
    }
}
