using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
public interface IUpdateCastMember : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
}
