namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
public class SearchOutput<TAggregate> where TAggregate : AggregateRoot
{
    public SearchOutput(
        int currentPage,
        int perPage,
        IReadOnlyList<TAggregate> items,
        int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Items = items;
        Total = total;
    }

    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public IReadOnlyList<TAggregate> Items { get; set; }
    public int Total { get; set; }
}
