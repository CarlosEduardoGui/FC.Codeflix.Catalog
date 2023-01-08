namespace FC.Codeflix.Catalog.Application.Common;
public abstract class PaginatedListOuput<TOutputItem>
{
    public PaginatedListOuput(
        int currentPage,
        int perPage,
        IReadOnlyList<TOutputItem> items,
        int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Items = items;
        Total = total;
    }

    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public IReadOnlyList<TOutputItem> Items { get; set; }
    public int Total { get; set; }
}