using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture : BaseFixture
{
    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {

        var genre = new Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public List<Genre> GetExampleListGenres(int count = 10)
        => Enumerable
            .Range(1, count)
            .Select(_ => GetExampleGenre())
            .ToList();

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
}
