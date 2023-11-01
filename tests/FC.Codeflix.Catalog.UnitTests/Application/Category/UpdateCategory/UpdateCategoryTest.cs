using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory;

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
    public async Task UpdateCategoryOk(CategoryEntity exampleCategory, UpdateCategoryInput input)
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
        outPut.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Trait("Use Cases", "UpdateCategory - Use Case")]
    [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActives))]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryWithoutProvidingIsActives(CategoryEntity exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name,
            exampleInput.Description
        );
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

        var outPut = await useCase.Handle(input, It.IsAny<CancellationToken>());

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
        outPut.IsActive.Should().Be(exampleCategory.IsActive!);
    }

    [Trait("Use Cases", "UpdateCategory - Use Case")]
    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryOnlyName(CategoryEntity exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name
        );
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

        var outPut = await useCase.Handle(input, It.IsAny<CancellationToken>());

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
        outPut.Description.Should().Be(exampleCategory.Description);
        outPut.IsActive.Should().Be(exampleCategory.IsActive!);
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

    [Trait("Use Cases", "UpdateCategory - Use Case")]
    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
    {
        var exampleCategory = _fixture.GetExampleCategory();
        input.Id = exampleCategory.Id;
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

        var task = async () =>
            await useCase.Handle(input, It.IsAny<CancellationToken>());

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);
        repositoryMock.Verify(x =>
            x.GetByIdAsync(
                input.Id,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
