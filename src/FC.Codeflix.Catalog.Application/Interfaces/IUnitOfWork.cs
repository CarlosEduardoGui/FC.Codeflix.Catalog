namespace FC.CodeFlix.Catalog.Application.Interfaces;
public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
    Task RollbackAsync(CancellationToken cancellationToken);
}
