using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Extension;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Extensions;

public class RatingExtensionTest
{
    [Trait("Domain", "Rating - Extensions")]
    [Theory(DisplayName = nameof(StringToRating))]
    [InlineData("ER", Rating.ER)]
    [InlineData("L", Rating.L)]
    [InlineData("10", Rating.Rate_10)]
    [InlineData("12", Rating.Rate_12)]
    [InlineData("14", Rating.Rate_14)]
    [InlineData("16", Rating.Rate_16)]
    [InlineData("18", Rating.Rate_18)]
    public void StringToRating(string enumString, Rating expectedRating)
    {
        enumString.ToRating().Should().Be(expectedRating);
    }

    [Trait("Domain", "Rating - Extensions")]
    [Fact(DisplayName = nameof(ThrowsExceptionWhenInvalidString))]
    public void ThrowsExceptionWhenInvalidString()
    {
        var action = () => "Invalid".ToRating();
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Trait("Domain", "Rating - Extensions")]
    [Theory(DisplayName = nameof(RatingToString))]
    [InlineData("ER", Rating.ER)]
    [InlineData("L", Rating.L)]
    [InlineData("10", Rating.Rate_10)]
    [InlineData("12", Rating.Rate_12)]
    [InlineData("14", Rating.Rate_14)]
    [InlineData("16", Rating.Rate_16)]
    [InlineData("18", Rating.Rate_18)]
    public void RatingToString(string expectedString, Rating rating)
    {
        rating.ToStringRating().Should().Be(expectedString);
    }
}
