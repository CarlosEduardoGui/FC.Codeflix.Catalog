using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
public record CreateVideoInput(
    string Title,
    string Description,
    bool Published,
    int Duration,
    Rating Rating,
    int YearLaunched,
    bool Opened,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null
) : IRequest<VideoModelOutput>;
