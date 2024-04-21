using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembers : IListCastMembers
{
    private readonly ICastMemberRepository _castMemberRepository;

    public ListCastMembers(ICastMemberRepository castMemberRepository)
        => _castMemberRepository = castMemberRepository;

    public async Task<ListCastMembersOutput> Handle(ListCastMembersInput request, CancellationToken cancellationToken)
    {
        var castMembers = await _castMemberRepository.SearchAsync(request.ToSearchInput(), cancellationToken);

        return ListCastMembersOutput.FromSearchOutput(castMembers);
    }
}
