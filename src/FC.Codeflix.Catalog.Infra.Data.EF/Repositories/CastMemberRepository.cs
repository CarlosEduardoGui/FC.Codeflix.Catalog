using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class CastMemberRepository : ICastMemberRepository
{
    private readonly CodeflixCatelogDbContext _dbContext;

    private DbSet<CastMember> _castMembers => _dbContext.Set<CastMember>();

    public CastMemberRepository(CodeflixCatelogDbContext dbContext)
        => _dbContext = dbContext;

    public Task DeleteAsync(CastMember aggregate, CancellationToken cancellationToken)
        => Task.FromResult(_castMembers.Remove(aggregate));

    public async Task<CastMember> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var castMember = await _castMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(
            castMember,
            string.Format(ConstantsMessages.CastMemberIdNotFound, id)
        );

        return castMember!;
    }

    public async Task InsertAsync(CastMember aggregate, CancellationToken cancellationToken)
        => await _castMembers.AddAsync(aggregate, cancellationToken);

    public async Task<SearchOutput<CastMember>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;

        var query = _castMembers.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.SearchOrder);

        if (string.IsNullOrWhiteSpace(input.Search) is false)
            query = query.Where(x => x.Name.Contains(input.Search));

        var castMembers = await query
            .AsNoTracking()
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var count = await query.CountAsync(cancellationToken);

        return new SearchOutput<CastMember>(
            input.Page,
            input.PerPage,
            castMembers,
            count
        );
    }

    public Task UpdateAsync(CastMember aggregate, CancellationToken cancellation)
        => Task.FromResult(_castMembers.Update(aggregate));

    private static IQueryable<CastMember> AddOrderToQuery(
        IQueryable<CastMember> query,
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
}
