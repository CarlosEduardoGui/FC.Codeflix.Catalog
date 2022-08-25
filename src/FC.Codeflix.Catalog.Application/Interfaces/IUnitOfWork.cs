namespace FC.Codeflix.Catalog.Application.Interfaces;
public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}
