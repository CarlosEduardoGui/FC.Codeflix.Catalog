using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Common;
public class CategoryPersistence
{
    private readonly CodeFlixCatelogDbContext _context;

    public CategoryPersistence(CodeFlixCatelogDbContext context) =>
        _context = context;

    public async Task<Category?> GetById(Guid id) =>
        await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
}
