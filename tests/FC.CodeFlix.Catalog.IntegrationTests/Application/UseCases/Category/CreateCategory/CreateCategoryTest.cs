using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CreateCategoryUseCase = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory.CreateCategory;
using UoW = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork.UnitOfWork;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategory))]
    public async void CreateCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new CreateCategoryUseCase(repository, unitOfWork);

        var input = _fixture.GetInput();

        var outPut = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext(true)
            .Categories.FindAsync(outPut.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().Be(outPut.CreatedAt);
        outPut.Should().NotBeNull();
        outPut.Id.Should().NotBeEmpty();
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be(input.Description);
        outPut.IsActive.Should().Be(input.IsActive);
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    public async void CreateCategoryWithOnlyName()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new CreateCategoryUseCase(repository, unitOfWork);

        var input = new CreateCategoryInput(_fixture.GetInput().Name);

        var outPut = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext(true)
            .Categories.FindAsync(outPut.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be("");
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(outPut.CreatedAt);
        outPut.Should().NotBeNull();
        outPut.Id.Should().NotBeEmpty();
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be("");
        outPut.IsActive.Should().Be(true);
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new CreateCategoryUseCase(repository, unitOfWork);

        var exampleInput = _fixture.GetInput();
        var input = new CreateCategoryInput(
            exampleInput.Name,
            exampleInput.Description
        );

        var outPut = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext(true)
            .Categories.FindAsync(outPut.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(outPut.CreatedAt);
        outPut.Should().NotBeNull();
        outPut.Id.Should().NotBeEmpty();
        outPut.Name.Should().Be(input.Name);
        outPut.Description.Should().Be(input.Description);
        outPut.IsActive.Should().Be(true);
        outPut.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiateCategory(
        CreateCategoryInput input,
        string exceptionMessage
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UoW(dbContext);
        var useCase = new CreateCategoryUseCase(repository, unitOfWork);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
        var dbCategoriesList = _fixture.CreateDbContext(true)
            .Categories
            .AsNoTracking()
            .ToList();
        dbCategoriesList.Should().HaveCount(0);
    }
}
