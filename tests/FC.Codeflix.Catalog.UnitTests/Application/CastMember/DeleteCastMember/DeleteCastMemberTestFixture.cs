using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.DeleteCastMember;

[CollectionDefinition(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberFixtureCollection : ICollectionFixture<DeleteCastMemberTestFixture> { }

public class DeleteCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
}
