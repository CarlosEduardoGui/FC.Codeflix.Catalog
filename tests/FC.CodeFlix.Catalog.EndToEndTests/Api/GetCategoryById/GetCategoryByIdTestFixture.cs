using FC.CodeFlix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.GetCategoryById;

[CollectionDefinition(nameof(GetCategoryByIdTestFixture))]
public class GetCategoryByIdTestFixtureCollection : ICollectionFixture<GetCategoryByIdTestFixture> { }

public class GetCategoryByIdTestFixture : CategoryBaseFixture
{

}
