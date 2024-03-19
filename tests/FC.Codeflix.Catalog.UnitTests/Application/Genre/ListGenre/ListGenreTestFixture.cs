using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
using Xunit;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.ListGenre;

[CollectionDefinition(nameof(ListGenreTestFixture))]
public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenreTestFixture> { }

public class ListGenreTestFixture : GenreUseCaseBaseFixture
{
    public List<GenreEntity> GetExampleGenresList(int length = 10)
    {
        var list = new List<GenreEntity>();
        for (int i = 0; i < length; i++)
            list.Add(GetExampleGenre(categoriesIds: GetRandomIdsList()));

        return list;
    }

    public ListGenresInput GetExampleInput()
    {
        var random = new Random();
        return new ListGenresInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.ASC : SearchOrder.DESC
        );
    }
}
