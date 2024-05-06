using FC.Codeflix.Catalog.Domain.ValueObject;
using FC.Codeflix.Catalog.UnitTests.Commom;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.ValueObject;
public class ImageTest : BaseFixture
{

    [Trait("Domain", "Image - ValueObject")]
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var path = Faker.Image.PicsumUrl();

        var image = new Image(path);

        image.Should().NotBeNull();
        image.Path.Should().Be(path);
    }

    [Trait("Domain", "Image - ValueObject")]
    [Fact(DisplayName = nameof(EqualsByProperts))]
    public void EqualsByProperts()
    {
        var path = Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(path);

        var isItEquals = image == sameImage;

        isItEquals.Should().BeTrue();
    }

    [Trait("Domain", "Image - ValueObject")]
    [Fact(DisplayName = nameof(DifferentByProperts))]
    public void DifferentByProperts()
    {
        var pathOne = Faker.Image.PicsumUrl();
        var pathTwo = Faker.Image.PicsumUrl();
        var image = new Image(pathOne);
        var differentImage = new Image(pathTwo);

        var isItEquals = image == differentImage;

        isItEquals.Should().BeFalse();
    }
}
