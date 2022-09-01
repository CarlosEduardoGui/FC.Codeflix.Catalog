using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
public class UpdateCategory : IUpdateCategory
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryModelOutput> Handle(UpdateCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        category.Update(request.Name, request.Description);

        if (request.IsActive != null && request.IsActive != category.IsActive)
            if ((bool)request.IsActive!)
                category.Activate();
            else
                category.Deactivate();

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}
