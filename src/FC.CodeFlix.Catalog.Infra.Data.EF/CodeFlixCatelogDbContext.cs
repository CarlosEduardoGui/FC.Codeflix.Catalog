using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class CodeflixCatelogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public CodeflixCatelogDbContext(
        DbContextOptions<CodeflixCatelogDbContext> options
    ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
    }
}
