using FC.Codeflix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }

public class ListCategoriesTestFixture : CategoryBaseFixture
{
}
