using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Infra.Data.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.Infra.Data.EF;
public class CodeFlixCatelogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public CodeFlixCatelogDbContext(
        DbContextOptions<CodeFlixCatelogDbContext> options
    ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
    }
}
