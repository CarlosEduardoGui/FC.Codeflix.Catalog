using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Genre : AggregateRoot
{
    public Genre(string name, bool isActive = true)
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));
    }

    public void Update(string newName)
    {
        Name = newName;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;

        Validate();
    }
    public void Deactivate()
    {
        IsActive = false;

        Validate();
    }

}
