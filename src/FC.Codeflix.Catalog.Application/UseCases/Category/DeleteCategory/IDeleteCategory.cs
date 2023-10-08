using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
public interface IDeleteCategory : IRequestHandler<DeleteCategoryInput>
{
}
