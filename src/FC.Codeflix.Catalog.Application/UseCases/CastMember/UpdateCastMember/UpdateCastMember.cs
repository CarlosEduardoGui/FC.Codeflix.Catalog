using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
public class UpdateCastMember : IUpdateCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCastMember(
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _castMemberRepository.GetByIdAsync(request.Id, cancellationToken);

        castMember.Update(request.Name, request.Type);

        await _castMemberRepository.UpdateAsync(castMember, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return CastMemberModelOutput.FromCastMember(castMember);
    }
}
