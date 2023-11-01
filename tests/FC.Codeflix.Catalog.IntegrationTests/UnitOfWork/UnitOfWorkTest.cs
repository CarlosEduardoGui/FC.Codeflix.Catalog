using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;

namespace FC.Codeflix.Catalog.IntegrationTests.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fxture) => _fixture = fxture;

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

        await unitOfWork.CommitAsync(CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var savedCategories = assertDbContext.Categories
            .AsNoTracking()
            .ToList();
        savedCategories.Should().HaveCount(exampleCategoriesList.Count);
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

        var task = async () => await unitOfWork.RollbackAsync(CancellationToken.None);

        await task.Should().NotThrowAsync();
    }
}
