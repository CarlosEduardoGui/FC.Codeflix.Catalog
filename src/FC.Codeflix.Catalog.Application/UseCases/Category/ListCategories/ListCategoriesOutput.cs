using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
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
