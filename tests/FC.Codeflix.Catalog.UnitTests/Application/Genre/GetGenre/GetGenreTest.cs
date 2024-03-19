using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
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
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: exampleCategories.Select(x => x.Id).ToList()
        );
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x =>
            x.GetListByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategories);
        var useCase = new UseCases.GetGenre(
            genreRepositoryMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCases.GetGenreInput(exampleGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleGenre.Categories.Count);
        foreach (var category in output.Categories)
        {
            var expectedCategory = exampleCategories.SingleOrDefault(x => x.Id == category.Id);
            exampleCategories.Should().NotBeNull();
            category.Name.Should().Be(expectedCategory!.Name);
        }
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
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var randomGenreId = Guid.NewGuid();
        genreRepositoryMock.Setup(x =>
            x.GetByIdAsync(It.Is<Guid>(x => x == randomGenreId), It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Genre '{randomGenreId}' not found."));
        var useCase = new UseCases.GetGenre(
            genreRepositoryMock.Object,
            categoryRepositoryMock.Object
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
