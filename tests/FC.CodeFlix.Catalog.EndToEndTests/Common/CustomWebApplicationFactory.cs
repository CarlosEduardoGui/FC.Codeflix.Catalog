using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FC.CodeFlix.Catalog.EndToEndTests.Common;
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbOptions = services.FirstOrDefault(
                x => x.ServiceType == typeof(
                    DbContextOptions<CodeFlixCatelogDbContext>
                )
            );

            if (dbOptions is not null)
                services.Remove(dbOptions);

            services.AddDbContext<CodeFlixCatelogDbContext>(options =>
            {
                options.UseInMemoryDatabase("endtoend-tests-db");
            });
        });

        base.ConfigureWebHost(builder);
    }
}
