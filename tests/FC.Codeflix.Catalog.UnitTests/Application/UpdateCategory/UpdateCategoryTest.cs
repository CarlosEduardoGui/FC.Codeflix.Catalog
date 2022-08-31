using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "UpdateCategory - Use Case")]
    [Theory(DisplayName = nameof(UpdateCategoryOk))]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryOk(Category exampleCategory, UseCase.UpdateCategoryInput input)
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.GetByIdAsync(
            exampleCategory.Id,
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        CategoryModelOutput outPut = await useCase.Handle(input, It.IsAny<CancellationToken>());

        repositoryMock.Verify(x =>
            x.GetByIdAsync(
                exampleCategory.Id,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(x =>
            x.UpdateAsync(
                exampleCategory,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        outPut.Should().NotBeNull();
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be(input.Description);
        outPut.IsActive.Should().Be(input.IsActive);
    }

    [Trait("Use Cases", "UpdateCategory - Use Case")]
    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput();
        repositoryMock.Setup(x => x.GetByIdAsync(
            input.Id,
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Category '{input.Id}' not found."));
        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async () =>
            await useCase.Handle(input, It.IsAny<CancellationToken>());

        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x =>
            x.GetByIdAsync(
                input.Id,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
