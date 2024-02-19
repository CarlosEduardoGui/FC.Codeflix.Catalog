using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;

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

    private void Validate()
    {

    }
}
