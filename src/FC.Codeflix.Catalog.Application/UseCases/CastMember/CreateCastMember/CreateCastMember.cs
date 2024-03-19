using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
public class CreateCastMember : ICreateCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCastMember(
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = new DomainEntity.CastMember(request.Name, request.Type);

        await _castMemberRepository.InsertAsync(castMember, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return CastMemberModelOutput.FromCastMember(castMember);
    }
}
