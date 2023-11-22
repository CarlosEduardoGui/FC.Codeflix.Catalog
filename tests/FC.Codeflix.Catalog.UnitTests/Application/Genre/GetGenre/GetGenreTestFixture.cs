using FC.Codeflix.Catalog.UnitTests.Application.Genre.Comon;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture> { }

public class GetGenreTestFixture : GenreUseCaseBaseFixture
{
}
