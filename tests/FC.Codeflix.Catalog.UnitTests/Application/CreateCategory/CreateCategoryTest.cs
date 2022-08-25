using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTest
{
    [Trait("Use Cases", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategory))]
    public async void CreateCategory()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCases.CreateCategoryInput(
            "Category Name",
            "Category Description",
            true
        );

        var outPut = await useCase.HandleAsync(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<Category>(),
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
}