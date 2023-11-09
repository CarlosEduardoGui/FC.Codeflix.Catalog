namespace FC.Codeflix.Catalog.Api.ApiModels.Category;

public class UpdateCategoryApiInput
{
    public UpdateCategoryApiInput(
        string name,
        string? description = null,
        bool? isActive = null)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
