using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Commom;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture> { }

public class CastMemberTestFixture : BaseFixture
{
    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)new Random().Next(1, 2);

    public DomainEntity.CastMember GetValidCastMember()
        => new(GetValidName(), GetRandomCastMemberType());
}
