using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryFixtureCollection : ICollectionFixture<CastMemberRepositoryTestFixture> { }

public class CastMemberRepositoryTestFixture : BaseFixture
{
    public CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());

    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)new Random().Next(1, 2);

    public List<CastMember> GetExampleCastMembersList(int quantity = 10)
    => Enumerable
        .Range(1, quantity)
        .Select(_ => GetExampleCastMember())
        .ToList();

    public List<CastMember> GetExampleCategoriesListWithNames(List<string> names) =>
    names.Select(name =>
    {
        var castMember = GetExampleCastMember();

        castMember.Update(name, castMember.Type);

        return castMember;
    }
    ).ToList();

    public List<CastMember> CloneCastMembersListOrdered(List<CastMember> castMemberList, string orderBy, SearchOrder order)
    {
        var listClone = new List<CastMember>(castMemberList);
        var orderEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.ASC) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.ASC) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.DESC) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.ASC) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.DESC) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        return orderEnumerable.ToList();
    }
}
