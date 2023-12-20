using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _uow;

    public CreateGenre(
        IGenreRepository repository,
        IUnitOfWork uow,
        ICategoryRepository categoryRepository
    )
    {
        _repository = repository;
        _uow = uow;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutPut> Handle(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = new DomainEntity.Genre(input.Name, input.IsAtive);

        if ((input.CategoriesIds?.Count ?? 0) > 0)
        {
            await ValidateCategoriesIds(input, cancellationToken);

            foreach (var categoryId in input.CategoriesIds!)
                genre.AddCategory(categoryId);
        }

        await _repository.InsertAsync(genre, cancellationToken);

        await _uow.CommitAsync(cancellationToken);

        return GenreModelOutPut.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var idsInPersistence = await _categoryRepository.GetIdsListByIdsAsync(input.CategoriesIds!, cancellationToken);

        if (idsInPersistence.Count < input.CategoriesIds!.Count)
        {
            var notFounds = input.CategoriesIds
                .FindAll(x => !idsInPersistence.Contains(x));

            var notFoundIdsAsString = string.Join(", ", notFounds);

            throw new RelatedAggregateException($"Related category Id (or Ids) not found: {notFoundIdsAsString}");
        }
    }
}
