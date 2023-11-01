using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
public class CreateCategory : ICreateCategory
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryModelOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken)
    {
        var category = new DomainEntity.Category(input.Name, input.Description, input.IsActive);

        await _categoryRepository.InsertAsync(category, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}
