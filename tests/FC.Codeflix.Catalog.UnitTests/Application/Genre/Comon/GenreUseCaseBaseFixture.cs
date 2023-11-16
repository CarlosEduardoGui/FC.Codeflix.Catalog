using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Commom;
using Moq;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Comon;
public class GenreUseCaseBaseFixture : BaseFixture
{
    public Mock<IGenreRepository> GetRepositoryMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();

    public GenreEntity GetExampleGenre(bool? isActive = null) =>
        new(GetValidGenreName(), isActive ?? GetRandomBoolean());

    public string GetValidGenreName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        return categoryName;
    }
}
