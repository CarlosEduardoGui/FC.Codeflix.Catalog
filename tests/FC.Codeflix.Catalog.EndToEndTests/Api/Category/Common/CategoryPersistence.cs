using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Entity.Category;


namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
public class CategoryPersistence
{
    private readonly CodeflixCatelogDbContext _context;

    public CategoryPersistence(CodeflixCatelogDbContext context) =>
        _context = context;

    public async Task<CategoryEntity?> GetByIdAsync(Guid id) =>
        await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertListAsync(List<CategoryEntity> categories)
    {
        await _context.Categories
            .AddRangeAsync(categories);

        await _context.SaveChangesAsync();
    }
}
