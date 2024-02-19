using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest
{
    private readonly CastMemberTestFixture _fixture;

    public CastMemberTest(CastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Domain", "CastMember - Aggregates")]
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var dateTimeBefore = DateTime.Now.AddSeconds(-1);
        var name = _fixture.GetValidName();
        var type = _fixture.GetRandomCastMemberType();

        var castMember = new DomainEntity.CastMember(name, type);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);
        castMember.Id.Should().NotBeEmpty();
        castMember.Name.Should().Be(name);
        castMember.CastMemberType.Should().Be(type);
        (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }
}
