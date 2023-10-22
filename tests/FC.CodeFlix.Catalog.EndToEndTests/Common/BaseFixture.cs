using Bogus;
using FC.CodeFlix.Catalog.EndToEndTests.Common;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base;
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
}
