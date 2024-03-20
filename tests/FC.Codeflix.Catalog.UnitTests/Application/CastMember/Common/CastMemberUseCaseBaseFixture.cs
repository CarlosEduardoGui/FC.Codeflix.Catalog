using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Commom;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
public class CastMemberUseCaseBaseFixture : BaseFixture
{
    public DomainEntity.CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());

    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)new Random().Next(1, 2);
}
