namespace FC.Codeflix.Catalog.Api.ApiModels.Genre;

public class UpdateGenreApiInput
{
    public UpdateGenreApiInput(string name, bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        Name = name;
        IsActive = isActive;
        CategoriesIds = categoriesIds;
    }

    public string Name { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? CategoriesIds { get; set; }
}
