using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
public class DeleteCastMember : IDeleteCastMember
{
    private readonly ICastMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCastMember(
        ICastMemberRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _repository.GetByIdAsync(request.Id, cancellationToken);

        await _repository.DeleteAsync(castMember, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
