using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using Xunit;


namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;

[CollectionDefinition(nameof(ListCastMemberTestFixture))]
public class ListCastMemberTestFixtureCollection : ICollectionFixture<ListCastMemberTestFixture> { }

public class ListCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
}
