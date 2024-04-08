using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "UpdateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = _fixture.GetExampleCastMember();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var input = new UpdateCastMemberInput(
            castMemberExample.Id,
            newName,
            newType
        );
        repositoryMock.Setup(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == castMemberExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(castMemberExample);
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        unitOfWorkMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        repositoryMock.Verify(x => x.UpdateAsync(
                It.Is<DomainEntity.CastMember>(
                    x => x.Name == input.Name
                    && x.Type == input.Type
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Trait("Use Cases", "UpdateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    public async Task ThrowsWhenNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var input = new UpdateCastMemberInput(
            Guid.NewGuid(),
            newName,
            newType
        );
        repositoryMock.Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new NotFoundException(""));
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>();
        unitOfWorkMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()
            ), Times.Never
        );
        repositoryMock.Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        repositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<DomainEntity.CastMember>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }

    [Trait("Use Cases", "UpdateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenInvalidName))]
    public async Task ThrowsWhenInvalidName()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = _fixture.GetExampleCastMember();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var input = new UpdateCastMemberInput(
            castMemberExample.Id,
            null!,
            newType
        );
        repositoryMock.Setup(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == castMemberExample.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(castMemberExample);
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
        unitOfWorkMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()
            ), Times.Never
        );
        repositoryMock.Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
        repositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<DomainEntity.CastMember>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }
}
