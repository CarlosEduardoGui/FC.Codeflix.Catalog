using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class CastMember : AggregateRoot
{
    public string Name { get; private set; }
    public CastMemberType CastMemberType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CastMember(string name, CastMemberType castMemberType)
    {
        Name = name;
        CastMemberType = castMemberType;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public void Update(string newName, CastMemberType newType)
    {
        Name = newName;
        CastMemberType = newType;

        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));
    }
}
