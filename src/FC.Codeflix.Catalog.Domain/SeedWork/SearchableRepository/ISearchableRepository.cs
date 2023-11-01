namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
public interface ISearchableRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<SearchOutput<TAggregate>> SearchAsync(SearchInput input, CancellationToken cancellationToken);
}