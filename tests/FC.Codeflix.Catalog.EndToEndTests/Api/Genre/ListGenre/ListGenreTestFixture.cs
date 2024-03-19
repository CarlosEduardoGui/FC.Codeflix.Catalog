using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenre;

[CollectionDefinition(nameof(ListGenreTestFixture))]
public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenreTestFixture> { }

public class ListGenreTestFixture : GenreBaseFixture
{
    public List<GenreEntity> GetExampleListGenresByNames(List<string> names)
    => names
       .Select(name =>
       {
           Task.Delay(5).GetAwaiter().GetResult();
           return GetExampleGenre(name: name);
       })
       .ToList();

    public List<GenreEntity> CloneGenresListOrdered(List<GenreEntity> genreList, string orderBy, SearchOrder order)
    {
        var listClone = new List<GenreEntity>(genreList);
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
