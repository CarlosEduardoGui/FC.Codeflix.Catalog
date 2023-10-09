using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catelog.Infra.Data.EF.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly CodeFlixCatelogDbContext _dbContext;
    private DbSet<Category> _categories =>
        _dbContext.Set<Category>();

    public CategoryRepository(
        CodeFlixCatelogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(Category aggregate, CancellationToken cancellationToken) =>
        await _categories.AddAsync(aggregate, cancellationToken);

    public Task DeleteAsync(Category aggregate, CancellationToken cancellationToken) => Task.FromResult(_categories.Remove(aggregate));

    public Task UpdateAsync(Category aggregate, CancellationToken cancellation) => Task.FromResult(_categories.Update(aggregate));

    public async Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
        );

        NotFoundException.ThrowIfNull(
            category,
            string.Format(ConstantsMessages.CategoryIdNotFound, id)
        );

        return category!;
    }

    public async Task<SearchOutput<Category>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        var total = await _categories.CountAsync();
        var items = await _categories.ToListAsync();
        return new(input.Page, input.PerPage, items, total);
    }
}