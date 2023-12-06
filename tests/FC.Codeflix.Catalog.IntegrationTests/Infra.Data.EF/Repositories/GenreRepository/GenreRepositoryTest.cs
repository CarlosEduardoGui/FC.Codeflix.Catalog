using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToList();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
        });
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var genre = await genreRepository.GetByIdAsync(exampleGenre.Id, CancellationToken.None);

        genre.Should().NotBeNull();
        genre!.Name.Should().Be(exampleGenre.Name);
        genre.IsActive.Should().Be(exampleGenre.IsActive);
        genre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genre.Categories.Should().HaveCount(categoriesListExample.Count);
        genre.Categories.ToList().ForEach(categoryId =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    public async Task GetThrowWhenNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGuid = Guid.NewGuid();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var action = async () => await genreRepository.GetByIdAsync(exampleGuid, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleGuid}' not found.");
    }

    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext
            .GenresCategories
            .AddRangeAsync(
                exampleGenre.Categories
                    .Select(categoryId =>
                        new GenresCategories(categoryId, exampleGenre.Id)
                    )
            );
        await dbContext.SaveChangesAsync();
        var repositoryDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        await genreRepository.DeleteAsync(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = assertsDbContext.Genres.AsNoTracking().FirstOrDefault(x => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();
        var categoriesIdsList = await dbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        categoriesIdsList.Should().HaveCount(0);
    }
}
