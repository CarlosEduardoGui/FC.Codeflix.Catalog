namespace FC.Codeflix.Catalog.Domain.SeedWork;
public interface IGenericRepository<TAggregate> : IRepository where TAggregate : AggregateRoot
{
    Task<TAggregate> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task InsertAsync(TAggregate aggregate, CancellationToken cancellationToken);
    Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken);
    Task UpdateAsync(TAggregate aggregate, CancellationToken cancellation);
}
