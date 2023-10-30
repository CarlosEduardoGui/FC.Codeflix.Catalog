using FC.CodeFlix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryFixture))]
public class DeleteCategoryFixtureCollection : ICollectionFixture<DeleteCategoryFixture> { }

public class DeleteCategoryFixture : CategoryBaseFixture
{
}
