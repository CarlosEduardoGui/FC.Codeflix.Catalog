using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Domain", "Genre - Aggregate")]
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var genreName = _fixture.GetValidName();

        var dateTimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(genreName);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        genre.Name.Should().Be(genreName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (genre.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }

    [Trait("Domain", "Genre - Aggregate")]
    [Theory(DisplayName = nameof(InstantiateThrowWhenNameEmpty))]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void InstantiateThrowWhenNameEmpty(string? name)
    {
        var action = () => new DomainEntity.Genre(name!);

        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Trait("Domain", "Genre - Aggregate")]
    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var genreName = _fixture.GetValidName();

        var dateTimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(genreName, isActive);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (genre.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }

    [Trait("Domain", "Genre - Aggregate")]
    [Theory(DisplayName = nameof(Activate))]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = _fixture.GetValidGenre(isActive);
        var oldName = genre.Name;

        genre.Activate();

        genre.IsActive.Should().BeTrue();
        genre.Name.Should().Be(oldName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Domain", "Genre - Aggregate")]
    [Theory(DisplayName = nameof(Deactivate))]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = _fixture.GetValidGenre(isActive);
        var oldName = genre.Name;

        genre.Deactivate();

        genre.IsActive.Should().BeFalse();
        genre.Name.Should().Be(oldName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Trait("Domain", "Genre - Aggregate")]
    [Theory(DisplayName = nameof(UpdateThrowWhenNameEmpty))]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void UpdateThrowWhenNameEmpty(string? name)
    {
        var genre = _fixture.GetValidGenre();

        var action = () => genre.Update(name!);

        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Trait("Domain", "Genre - Aggregate")]
    [Fact(DisplayName = nameof(Update))]
    public void Update()
    {
        var genre = _fixture.GetValidGenre();
        var newGenreName = _fixture.GetValidName();
        var oldIsActive = genre.IsActive;

        genre.Update(newGenreName);

        genre.Name.Should().Be(newGenreName);
        genre.IsActive.Should().Be(oldIsActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
