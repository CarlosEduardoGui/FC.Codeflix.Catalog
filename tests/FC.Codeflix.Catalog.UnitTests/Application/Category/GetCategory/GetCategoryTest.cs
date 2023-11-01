using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "GetCategory - Use Cases")]
    [Fact(DisplayName = nameof(GetCategory))]
    public async void GetCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleCategory = _fixture.GetExampleCategory();
        repositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleCategory);
        var input = new GetCategoryInput(exampleCategory.Id);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        var outPut = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
        ), Times.Once);
        outPut.Should().NotBeNull();
        outPut.Name.Should().Be(exampleCategory.Name);
        outPut.Description.Should().Be(exampleCategory.Description);
        outPut.IsActive.Should().Be(exampleCategory.IsActive);
        outPut.Id.Should().Be(exampleCategory.Id);
        outPut.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Trait("Use Cases", "GetCategory - Use Cases")]
    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExist))]
    public async void NotFoundExceptionWhenCategoryDoesntExist()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x =>
            x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
        )).ThrowsAsync(
            new NotFoundException($"Category {exampleGuid} not found.")
        );
        var input = new GetCategoryInput(exampleGuid);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x =>
            x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
