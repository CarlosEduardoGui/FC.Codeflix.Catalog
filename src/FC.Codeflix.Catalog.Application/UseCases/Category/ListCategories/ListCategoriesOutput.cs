using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategoriesOutput : PaginatedListOuput<CategoryModelOutput>
{
    public ListCategoriesOutput(
        int currentPage,
        int perPage,
        IReadOnlyList<CategoryModelOutput> items,
        int total) : base(currentPage, perPage, items, total)
    {
    }
}
