using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenreById;

[CollectionDefinition(nameof(GetGenreByIdTestFixture))]
public class GetGenreByIdTestFixtureCollection : ICollectionFixture<GetGenreByIdTestFixture> { }

public class GetGenreByIdTestFixture : GenreBaseFixture
{
}
