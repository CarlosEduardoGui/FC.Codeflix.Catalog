using Moq;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Entity;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.CreateCategory;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.UnitTests.UseCases.CreateCategory;
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
        var input = new CreateCategoryInput(
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
            unitOfWork => unitOfWork.Commit(
                It.IsAny<Category>(), 
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        outPut.Should().NotBeNull();
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be(input.Description);
        outPut.IsActive.Should().Be(input.Category);
        (outPut.Id != null && outPut.Id != default).Should().BeTrue();
        (outPut.CreatedAt != default).Should().BeTrue();
    }
}
