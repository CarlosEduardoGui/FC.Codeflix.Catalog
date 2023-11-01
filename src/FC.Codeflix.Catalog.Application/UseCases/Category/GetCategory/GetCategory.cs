using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
public class GetCategory : IGetCategory
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategory(ICategoryRepository categoryRepository) => _categoryRepository = categoryRepository;

    public async Task<CategoryModelOutput> Handle(GetCategoryInput input, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(input.Id, cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}
