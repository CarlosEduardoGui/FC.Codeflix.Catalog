using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UoW = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork.UnitOfWork;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    [Fact(DisplayName = nameof(DeleteCategoryOk))]
    public async Task DeleteCategoryOk()
    {
        var dbContext = _fixture.CreateDbContext();
        var categoryExample = _fixture.GetExampleCategory();
        var exampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleList);
        var tracking = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.DeleteCategory(
            repository,
            unitOfWork
        );
        var input = new DeleteCategoryInput(categoryExample.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbCategoryDeleted = await assertDbContext.Categories.FindAsync(categoryExample.Id);
        dbCategoryDeleted.Should().BeNull();
        var dbCategories = await assertDbContext.Categories.ToListAsync();
        dbCategories.Should().HaveCount(exampleList.Count);
    }

    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    public async void ThrowWhenCategoryNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var categoryExample = _fixture.GetExampleCategory();
        var exampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleList);
        var tracking = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.DeleteCategory(
            repository,
            unitOfWork
        );
        var input = new DeleteCategoryInput(Guid.NewGuid());

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }
}
