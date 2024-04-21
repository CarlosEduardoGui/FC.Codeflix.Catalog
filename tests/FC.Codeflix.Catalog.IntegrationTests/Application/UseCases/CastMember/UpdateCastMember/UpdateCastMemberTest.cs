using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UoW = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork.UnitOfWork;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.UpdateCastMember;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class UpdateCastMemberTest
{
    private readonly CastMemberUseCaseBaseFixture _fixture;

    public UpdateCastMemberTest(CastMemberUseCaseBaseFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        var examples = _fixture.GetExampleCastMembersList();
        var exampleCastMember = examples[5];
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = _fixture.CastMemberRepository(actDbContext);
        var unitOfWork = new UoW(actDbContext);
        var useCase = new UseCase.UpdateCastMember(repository, unitOfWork);
        var input = new UpdateCastMemberInput(exampleCastMember.Id, newName, newType);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(newName);
        output.Type.Should().Be(newType);
        output.CreatedAt.Should().BeSameDateAs(exampleCastMember.CreatedAt);
        var item = _fixture
            .CreateDbContext(true)
            .CastMembers
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == exampleCastMember.Id);
        item.Should().NotBeNull();
        item!.Name.Should().Be(newName);
        item.Type.Should().Be(newType);
        item.CreatedAt.Should().BeSameDateAs(exampleCastMember.CreatedAt);
    }

    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(UpdateWhenNotFound))]
    public async Task UpdateWhenNotFound()
    {
        var examples = _fixture.GetExampleCastMembersList();
        var exampleCastMember = Guid.NewGuid();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = _fixture.CastMemberRepository(actDbContext);
        var unitOfWork = new UoW(actDbContext);
        var useCase = new UseCase.UpdateCastMember(repository, unitOfWork);
        var input = new UpdateCastMemberInput(exampleCastMember, newName, newType);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"CastMember '{exampleCastMember}' not found.");
    }
}
