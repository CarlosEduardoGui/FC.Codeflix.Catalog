using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture> { }

public class DeleteCategoryTestFixture : CategoryUseCaseBaseFixture
{
}
