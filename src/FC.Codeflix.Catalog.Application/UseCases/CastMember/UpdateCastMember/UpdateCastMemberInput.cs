using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
public class UpdateCastMemberInput : IRequest<CastMemberModelOutput>
{
    public UpdateCastMemberInput(Guid id, string name, CastMemberType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
}
