using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.UnitOfWork;

[CollectionDefinition(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkCollection : ICollectionFixture<UnitOfWorkTestFixture> { }

public class UnitOfWorkTestFixture : BaseFixture
{
}
