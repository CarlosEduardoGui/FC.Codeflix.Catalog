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

    public Task<SearchOutput<CastMember>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CastMember aggregate, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
