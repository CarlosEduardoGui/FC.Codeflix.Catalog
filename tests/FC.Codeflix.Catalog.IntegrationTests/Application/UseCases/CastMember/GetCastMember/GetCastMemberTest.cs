using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.GetCastMember;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class GetCastMemberTest
{
    private readonly CastMemberUseCaseBaseFixture _fixture;

    public GetCastMemberTest(CastMemberUseCaseBaseFixture fixture)
        => _fixture = fixture;

    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    [Fact(DisplayName = nameof(GetCastMember))]
    public async Task GetCastMember()
    {
        var castMemberExamples = _fixture.GetExampleCastMembersList();
        var exampleTaget = castMemberExamples[5];
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(castMemberExamples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = _fixture.CastMemberRepository(arrangeDbContext);
        var useCase = new UseCase.GetCastMember(repository);
        var input = new GetCastMemberInput(exampleTaget.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleTaget.Id);
        output.Name.Should().Be(exampleTaget.Name);
        output.Type.Should().Be(exampleTaget.Type);
        output.CreatedAt.Should().BeSameDateAs(exampleTaget.CreatedAt);
    }

    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    [Fact(DisplayName = nameof(GetCastMemberThrowsWhenNotFound))]
    public async Task GetCastMemberThrowsWhenNotFound()
    {
        var castMemberExamples = _fixture.GetExampleCastMembersList();
        var exampleTaget = Guid.NewGuid();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(castMemberExamples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = _fixture.CastMemberRepository(arrangeDbContext);
        var useCase = new UseCase.GetCastMember(repository);
        var input = new GetCastMemberInput(exampleTaget);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"CastMember '{exampleTaget}' not found.");
    }
}
