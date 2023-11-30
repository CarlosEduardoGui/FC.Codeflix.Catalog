using FluentAssertions;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Insert))]
    public async Task Insert()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(dbContext);

        await genreRepository.InsertAsync(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbCategory = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleGenre.Name);
        dbCategory.IsActive.Should().Be(exampleGenre.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToList();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
        });
    }
}
