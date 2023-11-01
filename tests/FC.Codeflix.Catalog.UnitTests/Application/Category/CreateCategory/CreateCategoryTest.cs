using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategory))]
    public async void CreateCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetInput();

        var outPut = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<CategoryEntity>(),
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
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be(input.Description);
        outPut.IsActive.Should().Be(input.IsActive);
        outPut.Id.Should().NotBeEmpty();
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Use Cases", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    public async void CreateCategoryWithOnlyName()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var inputOnlyName = new CreateCategoryInput(_fixture.GetValidCategoryName());

        var outPut = await useCase.Handle(inputOnlyName, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<CategoryEntity>(),
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
        outPut.Name.Should().Be(inputOnlyName.Name);
        outPut.Description.Should().Be(string.Empty);
        outPut.IsActive.Should().BeTrue();
        outPut.Id.Should().NotBeEmpty();
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Use Cases", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var inputOnlyNameAndDescription = new CreateCategoryInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        var outPut = await useCase.Handle(inputOnlyNameAndDescription, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<CategoryEntity>(),
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
        outPut.Name.Should().Be(inputOnlyNameAndDescription.Name);
        outPut.Description.Should().Be(inputOnlyNameAndDescription.Description);
        outPut.IsActive.Should().BeTrue();
        outPut.Id.Should().NotBeEmpty();
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Use Cases", "CreateCategory - Use Cases")]
    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 24,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiateCategory(CreateCategoryInput input, string exceptionMessage)
    {
        var useCase = new UseCases.CreateCategory(
            _fixture.GetRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object
        );

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }
}