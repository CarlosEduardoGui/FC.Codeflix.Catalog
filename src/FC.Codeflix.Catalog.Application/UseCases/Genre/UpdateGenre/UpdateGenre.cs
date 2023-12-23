using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
public class UpdateGenre : IUpdateGenre
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _repository;
    private readonly IUnitOfWork _uow;

    public UpdateGenre(
        IGenreRepository repository,
        IUnitOfWork uow,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _uow = uow;
        _categoryRepository = categoryRepository;
    }
    public async Task<GenreModelOutPut> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _repository.GetByIdAsync(request.Id, cancellationToken);

        genre.Update(request.Name);

        if (
            request.IsActive is not null
            && request.IsActive != genre.IsActive
        )
            if ((bool)request.IsActive) genre.Activate();
            else genre.Deactivate();

        if (request.CategoriesIds?.Count >= 0)
        {
            genre.RemoveAllCategories();

            if (request.CategoriesIds?.Count > 0)
            {
                await ValidateCategoriesIds(request, cancellationToken);
                request.CategoriesIds.ForEach(genre.AddCategory);
            }
        }

        await _repository.UpdateAsync(genre, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return GenreModelOutPut.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var IdsInPersistence = await _categoryRepository
            .GetIdsListByIdsAsync(
                request.CategoriesIds!,
                cancellationToken
            );

        if (IdsInPersistence.Count < request.CategoriesIds!.Count)
        {
            var notFoundIds = request.CategoriesIds
                .FindAll(x => !IdsInPersistence.Contains(x));
            var notFoundIdsAsString = string.Join(", ", notFoundIds);
            throw new RelatedAggregateException(
                $"Related category Id (or Ids) not found: {notFoundIdsAsString}"
            );
        }
    }
}
