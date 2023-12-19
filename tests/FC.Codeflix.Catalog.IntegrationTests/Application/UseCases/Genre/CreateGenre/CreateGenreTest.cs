using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CreateGenreUseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre.CreateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateGenre))]
    public async Task CreateGenre()
    {
        var input = _fixture.GetExampleInput();
        var actDbContext = _fixture.CreateDbContext();
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var createGenreUseCase = new CreateGenreUseCase(genreRepository, unitOfWork, categoryRepository);

        var output = await createGenreUseCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsAtive);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(0);
        var assertDbContext = _fixture.CreateDbContext(true);
        var genreDb = await assertDbContext.Genres.FindAsync(output.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(output.Id);
        genreDb.Name.Should().Be(output.Name);
        genreDb.IsActive.Should().Be(output.IsActive);
        genreDb.CreatedAt.Should().Be(output.CreatedAt);
        genreDb.Categories.Should().HaveCount(output.Categories.Count);
    }

    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelation))]
    public async Task CreateGenreWithCategoriesRelation()
    {
        var exampleListCategories = _fixture.GetExampleCategoriesList(5);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleListCategories);
        await arrangeDbContext.SaveChangesAsync();
        var input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleListCategories
                                .Select(x => x.Id)
                                .ToList();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var createGenreUseCase = new CreateGenreUseCase(genreRepository, unitOfWork, categoryRepository);

        var output = await createGenreUseCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsAtive);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(input.CategoriesIds.Count);
        output.Categories.Select(relation => relation.Id).ToList()
            .Should().BeEquivalentTo(input.CategoriesIds);
        var assertDbContext = _fixture.CreateDbContext(true);
        var genreDb = await assertDbContext.Genres.FindAsync(output.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(output.Id);
        genreDb.Name.Should().Be(output.Name);
        genreDb.IsActive.Should().Be(output.IsActive);
        genreDb.CreatedAt.Should().Be(output.CreatedAt);
        var relations = await assertDbContext.GenresCategories
                            .AsNoTracking()
                            .Where(x => x.GenreId == output.Id)
                            .ToListAsync();
        relations.Should().HaveCount(input.CategoriesIds.Count);
        relations.Select(relation => relation.CategoryId).ToList()
            .Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesNotExist))]
    public async Task CreateGenreThrowsWhenCategoryDoesNotExist()
    {
        var exampleListCategories = _fixture.GetExampleCategoriesList(5);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleListCategories);
        await arrangeDbContext.SaveChangesAsync();
        var input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleListCategories
                                .Select(x => x.Id)
                                .ToList();
        var randomGuid = Guid.NewGuid();
        input.CategoriesIds.Add(randomGuid);
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var categoryRepository = _fixture.CreateCategoryRepository(actDbContext);
        var genreRepository = _fixture.GenreRepository(actDbContext);
        var createGenreUseCase = new CreateGenreUseCase(genreRepository, unitOfWork, categoryRepository);

        var action = async () => await createGenreUseCase.Handle(input, CancellationToken.None);


        await action.Should()
            .ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related category Id (or Ids) not found: {randomGuid}");
    }
}
