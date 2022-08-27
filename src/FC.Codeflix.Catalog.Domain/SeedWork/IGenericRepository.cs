namespace FC.Codeflix.Catalog.Domain.SeedWork;
public interface IGenericRepository<TAggregate> : IRepository
{
    Task<TAggregate> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task InsertAsync(TAggregate aggregate, CancellationToken cancellationToken);
}
