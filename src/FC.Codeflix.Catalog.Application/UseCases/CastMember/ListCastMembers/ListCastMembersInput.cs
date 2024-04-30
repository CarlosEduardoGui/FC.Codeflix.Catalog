using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembersInput : PaginatedListInput, IRequest<ListCastMembersOutput>
{
    public ListCastMembersInput() : base(1, 10, "", "", SearchOrder.ASC)
    {

    }

    public ListCastMembersInput(
        int page,
        int perPage,
        string search,
        string sort,
        SearchOrder dir) : base(page, perPage, search, sort, dir)
    {
    }
}
