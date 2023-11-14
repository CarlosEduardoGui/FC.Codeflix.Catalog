using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenreInput : IRequest<GenreModelOutPut>
{
    public CreateGenreInput(string name, bool isAtive, List<Guid>? categoriesIds = null)
    {
        Name = name;
        IsAtive = isAtive;
        CategoriesIds = categoriesIds;
    }

    public string Name { get; set; }
    public bool IsAtive { get; set; }
    public List<Guid>? CategoriesIds { get; set; }
}
