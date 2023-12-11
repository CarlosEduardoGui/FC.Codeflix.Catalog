using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
public interface IListGenres : IRequestHandler<ListGenresInput, ListGenresOutPut>
{
}
