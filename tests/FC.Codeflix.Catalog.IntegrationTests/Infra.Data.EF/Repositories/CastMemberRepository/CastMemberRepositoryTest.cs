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
        castMemberExample.Name.Should().Be(castMemberExample.Name);
        castMemberExample.Type.Should().Be(castMemberExample.Type);
    }
}
