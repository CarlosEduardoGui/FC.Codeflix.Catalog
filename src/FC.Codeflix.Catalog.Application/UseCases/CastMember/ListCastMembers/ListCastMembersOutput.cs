using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembersOutput : PaginatedListOuput<CastMemberModelOutput>
{
    public ListCastMembersOutput(
        int currentPage,
        int perPage,
        IReadOnlyList<CastMemberModelOutput> items,
        int total) : base(currentPage, perPage, items, total)
    {
    }
}
