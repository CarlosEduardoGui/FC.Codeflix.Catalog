using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
public interface IListCategories : IRequestHandler<ListCategoriesInput, ListCategoriesOutput>
{
}
