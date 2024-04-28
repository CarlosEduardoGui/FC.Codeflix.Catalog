using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Api.ApiModels.CastMember;

public class UpdateCastMemberApiInput
{
    public UpdateCastMemberApiInput(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; set; }
    public CastMemberType Type { get; set; }
}
