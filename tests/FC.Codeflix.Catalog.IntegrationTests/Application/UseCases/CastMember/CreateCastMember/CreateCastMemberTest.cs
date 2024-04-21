using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "CreateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(CreateCastMember))]
    public async Task CreateCastMember()
    {
        var actDbContext = _fixture.CreateDbContext();
        var repository = _fixture.CastMemberRepository(actDbContext);
        var unitOfWork = _fixture.CreateUnitOfWork(actDbContext);
        var useCase = new UseCase.CreateCastMember(repository, unitOfWork);
        var input = new CreateCastMemberInput(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        var assertDbContext = _fixture.CreateDbContext(true);
        var castMembersFromDb = assertDbContext.CastMembers.AsNoTracking().ToList();
        castMembersFromDb.Should().HaveCount(1);
        var castMember = castMembersFromDb[0];
        castMember.Should().NotBeNull();
        castMember.Id.Should().Be(output.Id);
        castMember.Name.Should().Be(output.Name);
        castMember.Type.Should().Be(output.Type);
        castMember.CreatedAt.Should().BeSameDateAs(output.CreatedAt);
    }
}
