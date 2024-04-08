using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "CreateCastMember - Use Cases")]
    [Fact(DisplayName = nameof(Create))]
    public async Task Create()
    {
        var input = new CreateCastMemberInput(
            _fixture.GetValidName(),
            _fixture.GetRandomCastMemberType()
        );
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        unitOfWorkMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.InsertAsync(
                It.Is<DomainEntity.CastMember>(x => x.Name == input.Name && x.Type == input.Type),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Trait("Use Cases", "CreateCastMember - Use Cases")]
    [Theory(DisplayName = nameof(ThrowsWhenInvalidName))]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task ThrowsWhenInvalidName(string invalidName)
    {
        var input = new CreateCastMemberInput(
            invalidName,
            _fixture.GetRandomCastMemberType()
        );
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
        unitOfWorkMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        repositoryMock.Verify(x => x.InsertAsync(
                It.Is<DomainEntity.CastMember>(x => x.Name == input.Name && x.Type == input.Type),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }
}
