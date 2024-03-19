using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Commom;
using Moq;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCaseBaseFixture : BaseFixture
{
    public Mock<IGenreRepository> GetRepositoryMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();

    public GenreEntity GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {

        var genre = new GenreEntity(GetValidGenreName(), isActive ?? GetRandomBoolean());

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public List<Guid> GetRandomIdsList(int? count = null) =>
        Enumerable
            .Range(1, count ?? (new Random().Next(1, 10)))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public string GetValidGenreName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        return categoryName;
    }

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
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription =
                categoryDescription[..10_000];
        return categoryDescription;
    }

    public CategoryEntity GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public List<CategoryEntity> GetExampleCategoriesList(int count = 5)
       => Enumerable.Range(0, count).Select(_ => GetExampleCategory())
           .ToList();
}
