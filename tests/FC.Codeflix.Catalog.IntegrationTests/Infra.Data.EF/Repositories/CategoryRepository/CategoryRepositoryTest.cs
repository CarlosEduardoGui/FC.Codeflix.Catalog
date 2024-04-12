using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

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
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
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
    [Fact(DisplayName = nameof(ListByIds))]
    public async Task ListByIds()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        var targetCategories = Enumerable.Range(1, 3).Select(_ =>
        {
            var index = new Random().Next(0, exampleCategoriesList.Count - 1);
            return exampleCategoriesList[index].Id;
        }).Distinct().ToList();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        await dbContext.SaveChangesAsync();

        var output = await categoryRepository.GetListByIdsAsync(targetCategories, CancellationToken.None);

        output.Should().NotBeNull();
        output.Should().HaveCount(targetCategories.Count);
        foreach (Category outPutItem in output)
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
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
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

    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();

        var output = await categoryRepository.SearchAsync(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(quantityCategoriesToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
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
    [Theory(DisplayName = nameof(SearchByText))]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Sci-fi", 1, 5, 5, 6)]
    [InlineData("Sci-fi", 1, 2, 2, 6)]
    [InlineData("Sci-fi", 2, 5, 1, 6)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
    )
    {
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Future",
            "Sci-fi",
            "Sci-fi Robots",
            "Sci-fi StarWars",
            "Sci-fi StarTrek"
        });
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.ASC);
        await dbContext.SaveChangesAsync();

        var output = await categoryRepository.SearchAsync(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
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
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderby,
        string order
    )
    {
        CodeflixCatelogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var output = await categoryRepository.SearchAsync(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
            exampleCategoriesList,
            orderby,
            searchOrder
        );
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem!.Id.Should().Be(expectedItem!.Id);
            outPutItem.Name.Should().Be(expectedItem.Name);
            outPutItem.Description.Should().Be(expectedItem.Description);
            outPutItem.IsActive.Should().Be(expectedItem.IsActive);
            outPutItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
