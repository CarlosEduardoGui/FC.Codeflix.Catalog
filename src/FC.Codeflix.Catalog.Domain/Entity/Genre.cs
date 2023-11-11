using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Genre : AggregateRoot
{
    public Genre(string name, bool isActive = true)
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
    }

    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void Update(string newName) => Name = newName;
}
