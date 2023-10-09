using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catelog.Infra.Data.EF;
using FluentAssertions;
using Xunit;
using Repository = FC.CodeFlix.Catelog.Infra.Data.EF.Repositories;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTest
{
    private readonly CategoryRepositoryTestFixture _fixture;

    public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture) => _fixture = fixture;

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(Insert))]
    public async Task Insert()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.InsertAsync(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        CodeFlixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var dbCategory = await categoryRepository.GetByIdAsync(
            exampleCategory.Id,
            CancellationToken.None
        );

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(exampleCategory.Id);
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        CodeFlixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var newCategoryValues = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);
        await categoryRepository.UpdateAsync(
            exampleCategory,
            CancellationToken.None
        );
        await dbContext.SaveChangesAsync();

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(exampleCategory.Id);
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(GetThrowIfNotFound))]
    public async Task GetThrowIfNotFound()
    {
        CodeFlixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var task = async () => await categoryRepository.GetByIdAsync(
            exampleId,
            CancellationToken.None
        );

        await task.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{exampleId}' not found.");
    }
}
