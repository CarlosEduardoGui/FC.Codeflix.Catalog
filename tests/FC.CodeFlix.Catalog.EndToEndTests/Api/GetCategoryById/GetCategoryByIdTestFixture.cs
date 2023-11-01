using FC.Codeflix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.GetCategoryById;

[CollectionDefinition(nameof(GetCategoryByIdTestFixture))]
public class GetCategoryByIdTestFixtureCollection : ICollectionFixture<GetCategoryByIdTestFixture> { }

public class GetCategoryByIdTestFixture : CategoryBaseFixture
{

}
