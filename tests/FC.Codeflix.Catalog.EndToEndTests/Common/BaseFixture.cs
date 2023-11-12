using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;
public class BaseFixture
{
    protected Faker Faker { get; set; }

    public ApiClient ApiClient { get; set; }

    public CustomWebApplicationFactory<Program> WebApplicationFactory { get; set; }

    public HttpClient HttpClient { get; set; }

    private readonly string _dbConnectionString;

    protected BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebApplicationFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebApplicationFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);

        var configuration = WebApplicationFactory.Services.GetService(typeof(IConfiguration));

        ArgumentNullException.ThrowIfNull(configuration);

        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb");
    }


    public CodeflixCatelogDbContext CreateDbContext() => new(
                new DbContextOptionsBuilder<CodeflixCatelogDbContext>()
                  .UseSqlServer(
                        _dbConnectionString
                    )
                  .Options
            );

    public void CleanPersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

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
