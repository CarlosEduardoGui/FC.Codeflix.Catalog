using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Common;
public class CategoryPersistence
{
    private readonly CodeflixCatelogDbContext _context;

    public CategoryPersistence(CodeflixCatelogDbContext context) =>
        _context = context;

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertListAsync(List<Category> categories)
    {
        await _context.Categories
            .AddRangeAsync(categories);

        await _context.SaveChangesAsync();
    }
}
