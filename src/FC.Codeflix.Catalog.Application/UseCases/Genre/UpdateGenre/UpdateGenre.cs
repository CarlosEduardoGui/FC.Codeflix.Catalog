using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
public class UpdateGenre : IUpdateGenre
{
    private readonly IGenreRepository _repository;
    private readonly IUnitOfWork _uow;

    public UpdateGenre(
        IGenreRepository repository,
        IUnitOfWork uow
    )
    {
        _repository = repository;
        _uow = uow;
    }
    public async Task<GenreModelOutPut> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _repository.GetByIdAsync(request.Id, cancellationToken);

        genre.Update(request.Name);
        if (request.IsActive is not null && request.IsActive != genre.IsActive)
            if ((bool)request.IsActive) genre.Activate();
            else genre.Deactivate();

        await _repository.UpdateAsync(genre, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return GenreModelOutPut.FromGenre(genre);
    }
}
