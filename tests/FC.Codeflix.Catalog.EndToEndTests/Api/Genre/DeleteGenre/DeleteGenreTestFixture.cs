using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteCategory;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }

public class DeleteGenreTestFixture : GenreBaseFixture
{
}
