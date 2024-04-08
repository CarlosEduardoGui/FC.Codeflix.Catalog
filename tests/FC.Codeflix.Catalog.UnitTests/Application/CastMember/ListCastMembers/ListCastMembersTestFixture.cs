using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using Xunit;
using Entity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.ListCastMembers;

[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMemberFixtureCollection : ICollectionFixture<ListCastMembersTestFixture> { }

public class ListCastMembersTestFixture : CastMemberUseCaseBaseFixture
{
    public List<Entity.CastMember> GetExampleCastMembersList(int quantity)
    => Enumerable
        .Range(1, quantity)
        .Select(_ => GetExampleCastMember())
        .ToList();
}
