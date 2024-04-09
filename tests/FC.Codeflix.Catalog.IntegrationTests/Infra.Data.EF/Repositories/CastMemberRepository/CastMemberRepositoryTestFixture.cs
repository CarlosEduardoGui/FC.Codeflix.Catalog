using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;


namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryFixtureCollection : ICollectionFixture<CastMemberRepositoryTestFixture> { }

public class CastMemberRepositoryTestFixture : BaseFixture
{
    public DomainEntity.CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());

    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)new Random().Next(1, 2);

    public List<DomainEntity.CastMember> GetExampleCastMembersList(int quantity)
    => Enumerable
        .Range(1, quantity)
        .Select(_ => GetExampleCastMember())
        .ToList();
}
