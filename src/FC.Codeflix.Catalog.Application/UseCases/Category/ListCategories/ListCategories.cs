using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategories : IListCategories
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategories(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ListCategoriesOutput> Handle(ListCategoriesInput request, CancellationToken cancellationToken)
    {
        var searchOuput = await _categoryRepository.SearchAsync(
            new(request.Page, request.PerPage, request.Search, request.Sort, request.Dir),
            cancellationToken);


        return new ListCategoriesOutput(searchOuput.CurrentPage, searchOuput.PerPage,
            searchOuput.Items.Select(CategoryModelOutput.FromCategory).ToList(),
            searchOuput.Total);
    }
}
