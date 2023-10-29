using Bogus;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.EndToEndTests.Common;
public class BaseFixture
{
    protected BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebApplicationFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebApplicationFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
    }

    protected Faker Faker { get; set; }

    public ApiClient ApiClient { get; set; }

    public CustomWebApplicationFactory<Program> WebApplicationFactory { get; set; }

    public HttpClient HttpClient { get; set; }

    public CodeFlixCatelogDbContext CreateDbContext() => new(
                new DbContextOptionsBuilder<CodeFlixCatelogDbContext>()
                  .UseInMemoryDatabase("endtoend-tests-db")
                  .Options
            );

    public string GetInvalidNameTooLong()
    {
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";

        return tooLongNameForCategory;
    }

    public string GetInvalidDescriptionTooLong()
    {
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory =
                $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";

        return tooLongDescriptionForCategory;
    }

    public string GetInvalidNameTooShort() =>
        Faker.Commerce.ProductName()[..2];
}
