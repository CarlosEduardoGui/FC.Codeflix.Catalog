using FC.CodeFlix.Catalog.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.UnitOfWork;

[CollectionDefinition(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkCollection : ICollectionFixture<UnitOfWorkTestFixture> { }

public class UnitOfWorkTestFixture : BaseFixture
{
}
