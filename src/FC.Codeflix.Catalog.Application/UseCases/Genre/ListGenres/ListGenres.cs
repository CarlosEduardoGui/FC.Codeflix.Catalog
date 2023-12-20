using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
public class ListGenres : IListGenres
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenres(
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListGenresOutPut> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var listGenres = await _genreRepository.SearchAsync(request.ToSearchInput(), cancellationToken);

        var genresListOutput = ListGenresOutPut.FromSearchOutPut(listGenres);

        var relatedCategoriesIds = listGenres.Items
            .SelectMany(genre => genre.Categories)
            .Distinct()
            .ToList();

        if (relatedCategoriesIds.Count > 0)
        {
            var listCategories = await _categoryRepository
                .GetListByIdsAsync(relatedCategoriesIds, cancellationToken);

            genresListOutput.FillWithCategoryNames(listCategories);
        }

        return genresListOutput;
    }
}
