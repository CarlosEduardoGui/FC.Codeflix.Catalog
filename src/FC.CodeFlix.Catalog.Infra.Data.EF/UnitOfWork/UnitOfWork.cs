using FC.CodeFlix.Catalog.Application.Interfaces;

namespace FC.CodeFlix.Catalog.Infra.Data.EF.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly CodeFlixCatelogDbContext _codeFlixCatelogDbContext;

    public UnitOfWork(CodeFlixCatelogDbContext codeFlixCatelogDbContext) =>
        _codeFlixCatelogDbContext = codeFlixCatelogDbContext;

    public Task CommitAsync(CancellationToken cancellationToken) =>
        _codeFlixCatelogDbContext.SaveChangesAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
