using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
public class DeleteCategoryInput : IRequest
{
    public DeleteCategoryInput(Guid id) => Id = id;

    public Guid Id { get; set; }
}
