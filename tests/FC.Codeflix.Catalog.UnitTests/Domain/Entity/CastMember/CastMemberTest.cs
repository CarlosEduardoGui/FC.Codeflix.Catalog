using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

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
        castMember.Type.Should().Be(type);
        (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }

    [Trait("Domain", "CastMember - Aggregates")]
    [Theory(DisplayName = nameof(ThrowsErrorWhenNameIsInvalid))]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowsErrorWhenNameIsInvalid(string? name)
    {
        var dateTimeBefore = DateTime.Now.AddSeconds(-1);
        var type = _fixture.GetRandomCastMemberType();

        var action = () => new DomainEntity.CastMember(name!, type);

        action
            .Should()
            .ThrowExactly<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Trait("Domain", "CastMember - Aggregates")]
    [Fact(DisplayName = nameof(Update))]
    public void Update()
    {
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var castMember = _fixture.GetValidCastMember();

        castMember.Update(newName, newType);

        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
    }

    [Trait("Domain", "CastMember - Aggregates")]
    [Theory(DisplayName = nameof(UpdateThrowsErrorWhenNameIsInvalid))]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateThrowsErrorWhenNameIsInvalid(string invalidName)
    {
        var newType = _fixture.GetRandomCastMemberType();
        var castMember = _fixture.GetValidCastMember();

        var action = () => castMember.Update(invalidName, newType);

        action
            .Should()
            .ThrowExactly<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }
}
