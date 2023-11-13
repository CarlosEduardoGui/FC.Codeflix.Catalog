using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture) => _categoryTestFixture = categoryTestFixture;

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
        category.IsActive.Should().BeTrue();
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateWithActive))]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithActive(bool isActive)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
        category.IsActive.Should().Be(isActive);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () =>
        {
            DomainEntity.Category category = new DomainEntity.Category(name!, validCategory.Description);
        };

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(validCategory.Name, null!);

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>().WithMessage("Description should not be null.");

    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [MemberData(nameof(GetNamesErrorWhenNameIsLessThan3Characters), parameters: 10)]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>().WithMessage("Name should be at leats 3 characters long.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);

        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>().WithMessage("Name should be less or equal 255 characters long.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();
        while (invalidDescription.Length <= 10_000)
            invalidDescription =
                $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";

        Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters long.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(Activate))]
    public void Activate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(Deactivate))]
    public void Deactivate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(Update))]
    public void Update()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var categoryWithNewValues = _categoryTestFixture.GetValidCategory();

        validCategory.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

        Assert.Equal(categoryWithNewValues.Name, validCategory.Name);
        Assert.Equal(categoryWithNewValues.Description, validCategory.Description);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(UpdateOnlyName))]
    public void UpdateOnlyName()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var newName = _categoryTestFixture.GetValidCategory();
        var currentDescription = validCategory.Description;

        validCategory.Update(newName.Name);

        validCategory.Name.Should().Be(newName.Name);
        validCategory.Description.Should().Be(currentDescription);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () =>
            validCategory.Update(name!, _categoryTestFixture.GetValidCategoryDescription());

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [MemberData(nameof(GetNamesErrorWhenNameIsLessThan3Characters), parameters: 10)]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () =>
            validCategory.Update(invalidName, _categoryTestFixture.GetValidCategoryDescription());

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be at leats 3 characters long.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);

        Action action = () =>
            validCategory.Update(invalidName, "Category Ok Description");

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long.");
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();
        while (invalidDescription.Length <= 10_000)
            invalidDescription =
                $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";

        Action action = () =>
            validCategory.Update("Category Name", invalidDescription);

        var exception = Assert.Throws<EntityValidationException>(() => action());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters long.");
    }

    public static IEnumerable<object[]> GetNamesErrorWhenNameIsLessThan3Characters(int numberOfTests = 6)
    {
        var fixture = new CategoryTestFixture();
        for (int i = 0; i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[] { fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)] };
        }
    }
}