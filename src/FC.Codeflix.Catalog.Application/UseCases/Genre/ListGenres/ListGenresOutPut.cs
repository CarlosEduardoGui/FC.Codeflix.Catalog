using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
public class ListGenresOutPut : PaginatedListOuput<GenreModelOutPut>
{
    public ListGenresOutPut(
        int currentPage,
        int perPage,
        IReadOnlyList<GenreModelOutPut> items,
        int total) : base(currentPage, perPage, items, total)
    {
    }

    public static ListGenresOutPut FromSearchOutPut(SearchOutput<GenreEntity> searchOutPut) =>
        new(
            searchOutPut.CurrentPage,
            searchOutPut.PerPage,
            searchOutPut.Items
                .Select(GenreModelOutPut.FromGenre)
                .ToList(),
            searchOutPut.Total
        );

    public void FillWithCategoryNames(IReadOnlyList<Domain.Entity.Category> listCategories)
    {
        foreach (var categoryOutput in from item in Items
                                       from categoryOutput in item.Categories
                                       select categoryOutput)
        {
            categoryOutput.Name = listCategories?
                            .FirstOrDefault(category => category.Id == categoryOutput.Id)?
                            .Name;
        }
    }
}
