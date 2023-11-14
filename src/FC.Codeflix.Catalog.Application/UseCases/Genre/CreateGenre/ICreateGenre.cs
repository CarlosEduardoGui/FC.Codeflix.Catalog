using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
internal interface ICreateGenre : IRequestHandler<CreateGenreInput, GenreModelOutPut>
{
    new Task<GenreModelOutPut> Handle(CreateGenreInput input, CancellationToken cancellationToken);
}
