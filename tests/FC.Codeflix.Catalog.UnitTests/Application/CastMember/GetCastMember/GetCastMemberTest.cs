using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture _fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "GetCastMember - Use Cases")]
    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMemberExample = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x =>
            x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(castMemberExample);
        var input = new GetCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(castMemberExample.Name);
        output.Type.Should().Be(castMemberExample.Type);
        repositoryMock.Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Trait("Use Cases", "GetCastMember - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenCastMemberIdDoesNotExist))]
    public async Task ThrowsWhenCastMemberIdDoesNotExist()
    {
        var guidExample = Guid.NewGuid();
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x =>
            x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException("not found."));
        var input = new GetCastMemberInput(guidExample);
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>();
        repositoryMock.Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }
}
