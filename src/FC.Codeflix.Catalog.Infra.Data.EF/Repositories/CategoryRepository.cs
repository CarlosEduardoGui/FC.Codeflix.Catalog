using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly CodeflixCatelogDbContext _dbContext;
    private DbSet<Category> _categories =>
        _dbContext.Set<Category>();

    public CategoryRepository(CodeflixCatelogDbContext dbContext)
        => _dbContext = dbContext;

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
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _categories.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.SearchOrder);
        if (string.IsNullOrEmpty(input.Search) is not true)
            query = query.Where(x => x.Name.Contains(input.Search));

        var total = await query.CountAsync(cancellationToken: cancellationToken);
        var items = await query.AsNoTracking()
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken: cancellationToken);

        return new(input.Page, input.PerPage, items, total);
    }

    private static IQueryable<Category> AddOrderToQuery(
        IQueryable<Category> query,
        string orderProperty,
        SearchOrder order
    )
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.ASC) => query.OrderBy(x => x.Name).ThenBy(x => x.Id.ToString()),
            ("name", SearchOrder.DESC) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id.ToString()),
            ("id", SearchOrder.ASC) => query.OrderBy(x => x.Id.ToString()),
            ("id", SearchOrder.DESC) => query.OrderByDescending(x => x.Id.ToString()),
            ("createdat", SearchOrder.ASC) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.DESC) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id.ToString())
        };

        return orderedQuery;
    }

    public async Task<IReadOnlyList<Guid>> GetIdsListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        =>
        (
            await _categories
                .AsNoTracking()
                .Where(category => ids.Contains(category.Id))
                .Select(category => category.Id)
                .ToListAsync(cancellationToken)
        )
        .AsReadOnly();

    public async Task<IReadOnlyList<Category>> GetListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken) =>
        await _categories
                .AsNoTracking()
                .Where(category => ids.Contains(category.Id))
                .ToListAsync(cancellationToken);
}