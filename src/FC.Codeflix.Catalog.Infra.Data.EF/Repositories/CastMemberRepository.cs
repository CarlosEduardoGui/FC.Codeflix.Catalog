using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
susing Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class CastMemberRepository : ICastMemberRepository
{
    private readonly CodeflixCatelogDbContext _dbContext;

    private DbSet<CastMember> _castMembers => _dbContext.Set<CastMember>();

    public CastMemberRepository(CodeflixCatelogDbContext dbContext)
        => _dbContext = dbContext;

    public Task DeleteAsync(CastMember aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CastMember> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
