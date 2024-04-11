using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private readonly CastMemberRepositoryTestFixture _fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Insert))]
    public async Task Insert()
    {
        var castMemberExample = _fixture.GetExampleCastMember();
        var context = _fixture.CreateDbContext();
        var repository = new Repository.CastMemberRepository(context);

        await repository.InsertAsync(castMemberExample, CancellationToken.None);
        await context.SaveChangesAsync();

        var assertionContext = _fixture.CreateDbContext(true);
        var castMemberFromDb = assertionContext
            .CastMembers
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == castMemberExample.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(castMemberExample.Name);
        castMemberFromDb.Type.Should().Be(castMemberExample.Type);
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));

        var itemFromRepository = await repository.GetByIdAsync(
            castMemberExample.Id,
            CancellationToken.None
        );

        itemFromRepository.Should().NotBeNull();
        itemFromRepository!.Name.Should().Be(castMemberExample.Name);
        itemFromRepository.Type.Should().Be(castMemberExample.Type);
    }

    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    public async Task GetThrowsWhenNotFound()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberRandomGuid = Guid.NewGuid();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));

        var action = async () => await repository.GetByIdAsync(
            castMemberRandomGuid,
            CancellationToken.None
        );

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"CastMember '{castMemberRandomGuid}' not found.");
    }
}
