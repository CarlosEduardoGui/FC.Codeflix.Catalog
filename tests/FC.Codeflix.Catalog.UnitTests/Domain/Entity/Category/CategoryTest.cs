using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

public class CategoryTest
{
    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };
        var dateTimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var dateTimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.NotEqual(default, category.CreateAt);
        Assert.True(category.CreateAt > dateTimeBefore);
        Assert.True(category.CreateAt < dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateWithActive))]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithActive(bool isActive)
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };
        var dateTimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.NotEqual(default, category.CreateAt);
        Assert.True(category.CreateAt > dateTimeBefore);
        Assert.True(category.CreateAt < dateTimeAfter);
        Assert.Equal(isActive, category.IsActive);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        void action()
        {
            _ = new DomainEntity.Category(name!, "Category Description");
        }

        var exception = Assert.Throws<EntityValidationException>(() => action());

        Assert.Equal("Name should be not empty or null.", exception.Message);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        static void action()
        {
            _ = new DomainEntity.Category("Category Name", null!);
        }

        var exception = Assert.Throws<EntityValidationException>(() => action());

        Assert.Equal("Description should be not empty or null.", exception.Message);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        void action()
        {
            _ = new DomainEntity.Category(invalidName, "Category Ok Description");
        }

        var exception = Assert.Throws<EntityValidationException>(() => action());

        Assert.Equal("Name should be at leats 3 characters long.", exception.Message);
    }

    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        void action()
        {
            _ = new DomainEntity.Category(invalidName, "Category Ok Description");
        }

        var exception = Assert.Throws<EntityValidationException>(() => action());

        Assert.Equal("Name should be less or equal 255 characters long.", exception.Message);
    }


    [Trait("Domain", "Category - Aggregates")]
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        void action()
        {
            _ = new DomainEntity.Category("Category Name", invalidDescription);
        }

        var exception = Assert.Throws<EntityValidationException>(() => action());

        Assert.Equal("Description should be less or equal 10.000 characters long.", exception.Message);
    }
}