using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
public class UpdateGenreInput : IRequest<GenreModelOutPut>
{
    public UpdateGenreInput(Guid id, string name, bool? isActive = null, List<Guid>? categoriesListIds = null)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CategoriesIds = categoriesListIds;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? CategoriesIds { get; set; }
}
