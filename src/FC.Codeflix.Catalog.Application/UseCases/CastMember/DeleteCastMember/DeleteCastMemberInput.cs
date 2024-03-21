using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
public class DeleteCastMemberInput : IRequest<Unit>
{
    public Guid Id { get; private set; }

    public DeleteCastMemberInput(Guid id)
    {
        Id = id;
    }
}
