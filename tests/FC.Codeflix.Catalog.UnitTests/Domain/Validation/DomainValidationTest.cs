using Bogus;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Trait("Domain", "DomainValidation - Validation")]
    [Fact(DisplayName = nameof(NotNullOk))]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.NotNull(value, fieldName);

        action.Should().NotThrow();
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    public void NotNullThrowWhenNull()
    {
        string? value = null;
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.NotNull(value, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null.");
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be empty or null.");
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    public void NotNullOrEmptyOk()
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        var target = Faker.Commerce.ProductName();
        Action action = () =>
            DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().NotThrow<EntityValidationException>();
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Theory(DisplayName = nameof(MinLenghtThrowWhenLess))]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
    public void MinLenghtThrowWhenLess(string target, int minLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.MinLenght(target, minLenght, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at leats {minLenght} characters long.");
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Theory(DisplayName = nameof(MinLenghtOk))]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters: 10)]
    public void MinLenghtOk(string target, int minLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.MinLenght(target, minLenght, fieldName);

        action.Should().NotThrow();
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Theory(DisplayName = nameof(MaxLenghtThrowWhenGreater))]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
    public void MaxLenghtThrowWhenGreater(string target, int maxLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.MaxLenght(target, maxLenght, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal {maxLenght} characters long.");
    }

    [Trait("Domain", "DomainValidation - Validation")]
    [Theory(DisplayName = nameof(MinLenghtOk))]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
    public void MaxLenghtOk(string target, int minLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () =>
            DomainValidation.MaxLenght(target, minLenght, fieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length + new Random().Next(1, 20);

            yield return new object[] { example, minLenght };
        }
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMin(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length - new Random().Next(1, 5);

            yield return new object[] { example, minLenght };
        }
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLenght = example.Length - new Random().Next(1, 5);

            yield return new object[] { example, maxLenght };
        }
    }

    public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length + new Random().Next(0, 20);

            yield return new object[] { example, minLenght };
        }
    }
}