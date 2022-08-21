using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

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
}