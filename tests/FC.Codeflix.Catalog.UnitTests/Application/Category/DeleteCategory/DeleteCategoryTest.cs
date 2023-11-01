using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "DeleteCategory - Use Cases")]
    [Fact(DisplayName = nameof(DeleteCategoryOk))]
    public async void DeleteCategoryOk()
    {
        var repository = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var categoryExample = _fixture.GetExampleCategory();
        repository.Setup(x =>
            x.GetByIdAsync(categoryExample.Id, It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoryExample);
        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new UseCase.DeleteCategory(
            repository.Object,
            unitOfWork.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        repository.Verify(x =>
            x.GetByIdAsync(categoryExample.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        repository.Verify(x =>
            x.DeleteAsync(categoryExample, It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWork.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Trait("Use Cases", "DeleteCategory - Use Cases")]
    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    public async void ThrowWhenCategoryNotFound()
    {
        var repository = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repository.Setup(x =>
            x.GetByIdAsync(exampleGuid, It.IsAny<CancellationToken>())
        ).ThrowsAsync(
            new NotFoundException($"Category '{exampleGuid}' not found.")
        );
        var input = new DeleteCategoryInput(exampleGuid);
        var useCase = new UseCase.DeleteCategory(
            repository.Object,
            unitOfWork.Object
        );

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repository.Verify(x =>
            x.GetByIdAsync(exampleGuid, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
