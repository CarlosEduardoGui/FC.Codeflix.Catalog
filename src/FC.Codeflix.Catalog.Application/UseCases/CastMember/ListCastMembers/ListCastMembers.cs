using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembers : IListCastMembers
{
    private readonly ICastMemberRepository _castMemberRepository;

    public ListCastMembers(ICastMemberRepository castMemberRepository)
        => _castMemberRepository = castMemberRepository;

    public async Task<ListCastMembersOutput> Handle(ListCastMembersInput request, CancellationToken cancellationToken)
    {
        var searchInput = new SearchInput(
            request.Page,
            request.PerPage,
            request.Search,
            request.Sort,
            request.Dir
        );

        var castMembers = await _castMemberRepository.SearchAsync(searchInput, cancellationToken);

        var castMembersReadOnlyList = castMembers.Items
                .Select(castMember =>
                    CastMemberModelOutput.FromCastMember(castMember))
                .ToList()
                .AsReadOnly();

        return new ListCastMembersOutput(
            castMembers.CurrentPage,
            castMembers.PerPage,
            castMembersReadOnlyList,
            castMembers.Total
        );
    }
}
