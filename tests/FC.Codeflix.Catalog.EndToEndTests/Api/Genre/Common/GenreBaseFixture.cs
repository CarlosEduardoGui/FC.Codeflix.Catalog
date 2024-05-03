using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Common;
using Xunit;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

[CollectionDefinition(nameof(GenreBaseFixture))]
public class GenreBaseFixtureCollection : ICollectionFixture<GenreBaseFixture> { }

public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence;
    public CategoryPersistence CategoriesPersistence;

    public GenreBaseFixture() : base()
    {
        var dbContext = CreateDbContext();
        Persistence = new GenrePersistence(dbContext);
        CategoriesPersistence = new CategoryPersistence(dbContext);
    }

    public List<GenreEntity> GetExampleListGenres(int count = 10)
        => Enumerable
            .Range(1, count)
            .Select(_ =>
            {
                Task.Delay(5).GetAwaiter().GetResult();
                return GetExampleGenre();
            })
            .ToList();

    public List<CategoryEntity> GetExampleCategoriesList(int lengh = 10) =>
       Enumerable.Range(1, lengh)
       .Select(_ => GetExampleCategory()).ToList();

    public CategoryEntity GetExampleCategory() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            );

    public GenreEntity GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null,
      string? name = null)
    {

        var genre = new GenreEntity(
            name ?? GetValidGenreName(),
            isActive ?? GetRandomBoolean()
        );

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public string GetValidGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];

        return genreName;
    }

    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[..255];

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];

        return categoryDescription;
    }

}
