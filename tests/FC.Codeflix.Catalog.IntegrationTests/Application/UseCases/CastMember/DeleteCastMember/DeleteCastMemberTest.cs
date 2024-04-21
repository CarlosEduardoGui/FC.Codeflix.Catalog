using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UoW = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork.UnitOfWork;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.DeleteCastMember;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class DeleteCastMemberTest
{
    private readonly CastMemberUseCaseBaseFixture _fixture;

    public DeleteCastMemberTest(CastMemberUseCaseBaseFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        var example = _fixture.GetExampleCastMember();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(example);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = _fixture.CastMemberRepository(actDbContext);
        var unitOfWork = new UoW(actDbContext);
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
        var input = new DeleteCastMemberInput(example.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var list = await assertDbContext.CastMembers.AsNoTracking().ToListAsync();
        list.Should().HaveCount(0);
    }

    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    [Fact(DisplayName = nameof(DeleteThrowsWhenNotFound))]
    public async Task DeleteThrowsWhenNotFound()
    {
        var example = _fixture.GetExampleCastMember();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(example);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = _fixture.CastMemberRepository(actDbContext);
        var unitOfWork = new UoW(actDbContext);
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
        var randomId = Guid.NewGuid();
        var input = new DeleteCastMemberInput(randomId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomId}' not found.");
    }
}
