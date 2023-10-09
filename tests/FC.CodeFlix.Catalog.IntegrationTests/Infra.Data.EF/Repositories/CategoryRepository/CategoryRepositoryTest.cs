using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
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

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
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
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(_fixture.CreateDbContext(true));

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
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
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

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().Be(exampleCategory.Id);
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
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var task = async () => await categoryRepository.GetByIdAsync(
            exampleId,
            CancellationToken.None
        );

        await task.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{exampleId}' not found.");
    }

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        CodeFlixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.DeleteAsync(
            exampleCategory,
            CancellationToken.None
        );
        await dbContext.SaveChangesAsync();

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().BeNull();
    }

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    public async Task SearchReturnsListAndTotal()
    {
        CodeFlixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();

        var output = await categoryRepository.SearchAsync(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        foreach (Category outPutItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category => 
                category.Id == outPutItem.Id
            );
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(exampleItem!.Id);
            outPutItem.Name.Should().Be(exampleItem.Name);
            outPutItem.Description.Should().Be(exampleItem.Description);
            outPutItem.IsActive.Should().Be(exampleItem.IsActive);
            outPutItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        CodeFlixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);
        
        var output = await categoryRepository.SearchAsync(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }
}
