using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _uow;

    public CreateVideo(
        IVideoRepository repository,
        ICategoryRepository categoryRepository,
        IGenreRepository genreRepository,
        IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
    }

    public async Task<VideoModelOutput> Handle(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var video = new DomainEntity.Video(
            request.Title,
            request.Description,
            request.YearLaunched,
            request.Opened,
            request.Published,
            request.Duration,
            request.Rating
        );

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException(
                "There are validation errors.",
                validationHandler.Errors
            );

        if ((request.CategoriesIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await _categoryRepository.GetIdsListByIdsAsync(
                request.CategoriesIds!.ToList(),
                cancellationToken
            );

            if (persistenceIds.Count < request.CategoriesIds!.Count)
            {
                var notFoundIds = request.CategoriesIds
                    .ToList()
                    .FindAll(categoryId => persistenceIds.Contains(categoryId) is false);

                var stringNotFoundIds = string.Join(',', notFoundIds);

                throw new RelatedAggregateException($"Related category Id (or Ids) not found: {stringNotFoundIds}.");
            }

            request.CategoriesIds!
                .ToList()
                .ForEach(video.AddCategory);
        }

        if((request.GenresIds?.Count ?? 0) > 0)
        {
            request.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        await _repository.InsertAsync(video, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return VideoModelOutput.FromVideo(video);
    }
}
