using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Domain.Repository;
public interface ICategoryRepository : IGenericRepository<Category>, ISearchableRepository<Category>
{
    Task<IReadOnlyList<Guid>> GetIdsListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
}
