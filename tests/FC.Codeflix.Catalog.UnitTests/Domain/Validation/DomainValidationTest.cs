using Bogus;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Trait("Domain", "DomainValidation - Validation")]
    [Fact(DisplayName = nameof(NotNullOk))]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();

        Action action = () => 
            DomainValidation.NotNull(value, "Value");

        action.Should().NotThrow();
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Fact(DisplayName = nameof(NotNullOk))]
    public void NotNullThrowWhenTrue()
    {
        string? value = null;

        Action action = () =>
            DomainValidation.NotNull(value, "FieldName");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("FieldName should be not null.");
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        Action action = () =>
            DomainValidation.NotNullOrEmpty(target, "FieldName");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("FieldName should be not null or empty.");
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    public void NotNullOrEmptyOk()
    {
        var target = Faker.Commerce.ProductName();
        Action action = () =>
            DomainValidation.NotNullOrEmpty(target, "FieldName");

        action.Should().NotThrow<EntityValidationException>();
    }
}
