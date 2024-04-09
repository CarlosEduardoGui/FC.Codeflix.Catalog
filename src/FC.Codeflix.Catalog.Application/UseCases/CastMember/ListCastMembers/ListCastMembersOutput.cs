using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Entity = FC.Codeflix.Catalog.Domain.Entity;

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

    public static ListCastMembersOutput FromSearchOutput(SearchOutput<Entity.CastMember> searchOutput)
        => new(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Items
                .Select(castMember =>
                    CastMemberModelOutput.FromCastMember(castMember))
                .ToList()
                .AsReadOnly(),
            searchOutput.Total
        );
}
