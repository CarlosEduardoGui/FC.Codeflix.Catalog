using FC.Codeflix.Catalog.UnitTests.Commom;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture> { }

public class GenreTestFixture : BaseFixture
{
    public string GetValidName() => Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetValidGenre(bool isActive = true) => 
        new(GetValidName(), isActive);
}
