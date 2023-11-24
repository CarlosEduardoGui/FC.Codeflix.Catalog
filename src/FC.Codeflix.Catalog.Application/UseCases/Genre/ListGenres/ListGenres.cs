using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
public class ListGenres : IListGenres
{
    private readonly IGenreRepository _genreRepository;

    public ListGenres(IGenreRepository genreRepository)
        => _genreRepository = genreRepository;

    public async Task<ListGenresOutPut> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var genreList = await _genreRepository.SearchAsync(request.ToSearchInput(), cancellationToken);

        return ListGenresOutPut.FromSearchOutPut(genreList);
    }
}
