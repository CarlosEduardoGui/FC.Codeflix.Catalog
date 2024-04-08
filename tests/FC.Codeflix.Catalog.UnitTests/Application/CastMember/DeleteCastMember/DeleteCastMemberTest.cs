using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "DeleteCastMember - Use Cases")]
    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        var castMemberExample = _fixture.GetExampleCastMember();
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(castMemberExample);
        var input = new DeleteCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().NotThrowAsync();
        repositoryMock.Verify(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        repositoryMock.Verify(x => x.DeleteAsync(
            It.Is<DomainEntity.CastMember>(x => x.Id == input.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "DeleteCastMember - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    public async Task ThrowsWhenNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new NotFoundException(""));
        var input = new DeleteCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>();
        repositoryMock.Verify(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
