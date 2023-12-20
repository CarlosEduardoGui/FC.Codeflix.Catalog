using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategoryById;

[CollectionDefinition(nameof(GetCategoryByIdTestFixture))]
public class GetCategoryByIdTestFixtureCollection : ICollectionFixture<GetCategoryByIdTestFixture> { }

public class GetCategoryByIdTestFixture : CategoryBaseFixture
{

}
