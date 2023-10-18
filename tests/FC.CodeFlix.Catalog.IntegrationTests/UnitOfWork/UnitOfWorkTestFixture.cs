using FC.CodeFlix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.UnitOfWork;

[CollectionDefinition(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkCollection : ICollectionFixture<UnitOfWorkTestFixture> { }

public class UnitOfWorkTestFixture : BaseFixture
{
}
