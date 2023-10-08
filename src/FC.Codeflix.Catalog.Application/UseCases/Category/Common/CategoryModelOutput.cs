using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
public class CategoryModelOutput
{
    public CategoryModelOutput(Guid id, string name, string description, bool isActive, DateTime createAt)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createAt;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }


    public static CategoryModelOutput FromCategory(DomainEntity.Category category) =>
        new(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive,
            category.CreatedAt
        );

    internal object ToList()
    {
        throw new NotImplementedException();
    }
}
