namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
public interface ICreateCategory
{
    Task<CreateCategoryOutput> HandleAsync(CreateCategoryInput input, CancellationToken cancellationToken);
}
