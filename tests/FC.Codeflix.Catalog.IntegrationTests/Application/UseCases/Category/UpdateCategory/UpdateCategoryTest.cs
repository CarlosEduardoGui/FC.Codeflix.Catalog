using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;
using UoW = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork.UnitOfWork;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Integration/Application", "UpdateCategory - Use Case")]
    [Theory(DisplayName = nameof(UpdateCategoryOk))]
    [MemberData(
    nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
    parameters: 5,
    MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryOk(CategoryEntity exampleCategory, UpdateCategoryInput input)
    {
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

        CategoryModelOutput outPut = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext(true)
           .Categories.FindAsync(outPut.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive!);
        dbCategory.CreatedAt.Should().Be(outPut.CreatedAt);
        outPut.Should().NotBeNull();
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be(input.Description);
        outPut.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryWithoutIsActive(
            CategoryEntity exampleCategory,
            UpdateCategoryInput exampleInput
        )
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name,
            exampleInput.Description
        );
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext(true)
            .Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryOnlyName(
            CategoryEntity exampleCategory,
            UpdateCategoryInput exampleInput
    )
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name
        );
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext(true)
            .Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    public async Task UpdateThrowsWhenNotFoundCategory()
    {
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        dbContext.SaveChanges();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }

    [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateThrowsWhenCantInstantiateCategory(
            UpdateCategoryInput input,
            string expectedExceptionMessage
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategories);
        dbContext.SaveChanges();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new UseCase.UpdateCategory(repository, unitOfWork);
        input.Id = exampleCategories[0].Id;

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);
    }
}
