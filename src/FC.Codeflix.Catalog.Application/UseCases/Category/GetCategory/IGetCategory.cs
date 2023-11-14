using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
public interface IGetCategory : IRequestHandler<GetCategoryInput, CategoryModelOutput>
{
    new Task<CategoryModelOutput> Handle(GetCategoryInput input, CancellationToken cancellationToken);
}
