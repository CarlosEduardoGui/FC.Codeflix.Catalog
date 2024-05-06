using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private readonly VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture videoTestFixture)
        => _fixture = videoTestFixture;

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(InstantiateOk))]
    public void InstantiateOk()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedRating = _fixture.GetRandomRating();

        var video = new DomainEntity.Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration,
            expectedRating
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
        video.Rating.Should().Be(expectedRating);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(Update))]
    public void Update()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var video = _fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    public void ValidateWhenValidState()
    {
        var video = _fixture.GetValidVideo();
        var notificationHandler = new NotificationValidationHandler();

        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
        notificationHandler.Errors.Should().HaveCount(0);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(InvalidWhenHasErrors))]
    public void InvalidWhenHasErrors()
    {
        var invalidVideo = new DomainEntity.Video(
            _fixture.GetTooLongTitle(),
            _fixture.GetTooLongDescription(),
            _fixture.GetValidYearLauched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationHandler = new NotificationValidationHandler();

        invalidVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
        {
            new("Title should be less or equal 255 characters long."),
            new("Description should be less or equal 4000 characters long.")
        });
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(ValidateWhenVideoUpdateStillValidEntity))]
    public void ValidateWhenVideoUpdateStillValidEntity()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var video = _fixture.GetValidVideo();
        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );
        var notificationHandler = new NotificationValidationHandler();

        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
        notificationHandler.Errors.Should().HaveCount(0);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(InvalidateWhenVideoUpdateIsNotValidEntity))]
    public void InvalidateWhenVideoUpdateIsNotValidEntity()
    {
        var expectedTitle = _fixture.GetTooLongTitle();
        var expectedDescription = _fixture.GetTooLongDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var video = _fixture.GetValidVideo();
        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );
        var notificationHandler = new NotificationValidationHandler();

        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
        {
            new("Title should be less or equal 255 characters long."),
            new("Description should be less or equal 4000 characters long.")
        });
    }
}
