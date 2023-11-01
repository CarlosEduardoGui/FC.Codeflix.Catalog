using FC.Codeflix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatelogDbContext _codeFlixCatelogDbContext;

    public UnitOfWork(CodeflixCatelogDbContext codeFlixCatelogDbContext) =>
        _codeFlixCatelogDbContext = codeFlixCatelogDbContext;

    public Task CommitAsync(CancellationToken cancellationToken) =>
        _codeFlixCatelogDbContext.SaveChangesAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
