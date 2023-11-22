using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
public class GetGenre : IGetGenre
{
    private readonly IGenreRepository _genreRepository;

    public GetGenre(IGenreRepository genreRepository) =>
        _genreRepository = genreRepository;

    public async Task<GenreModelOutPut> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.GetByIdAsync(request.Id, cancellationToken);

        return GenreModelOutPut.FromGenre(genre);
    }
}
