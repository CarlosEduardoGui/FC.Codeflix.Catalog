using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
public class DeleteGenre : IDeleteGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _uniUnitOfWork;

    public DeleteGenre(
        IGenreRepository genreRepository,
        IUnitOfWork uniUnitOfWork)
    {
        _genreRepository = genreRepository;
        _uniUnitOfWork = uniUnitOfWork;
    }

    public async Task Handle(DeleteGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.GetByIdAsync(request.Id, cancellationToken);

        await _genreRepository.DeleteAsync(genre, cancellationToken);
        await _uniUnitOfWork.CommitAsync(cancellationToken);

    }
}
