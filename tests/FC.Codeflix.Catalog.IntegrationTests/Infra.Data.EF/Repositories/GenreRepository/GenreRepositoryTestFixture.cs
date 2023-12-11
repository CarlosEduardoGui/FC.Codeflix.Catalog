using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture : BaseFixture
{
    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null,
      string? name = null)
    {

        var genre = new Genre(
            name ?? GetValidGenreName(),
            isActive ?? GetRandomBoolean()
        );

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public List<Genre> GetExampleListGenresByNames(List<string> names)
     => names
        .Select(name => GetExampleGenre(name: name))
        .ToList();

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
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];

        return genreName;
    }

    public List<Genre> CloneGenresListOrdered(List<Genre> genreList, string orderBy, SearchOrder order)
    {
        var listClone = new List<Genre>(genreList);
        var orderEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.ASC) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.ASC) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.ASC) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.DESC) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        return orderEnumerable.ToList();
    }
}
