using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
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
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
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

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateThumb))]
    public void UpdateThumb()
    {
        var video = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        video.UpdateThumb(validImagePath);

        video.Thumb.Should().NotBeNull();
        video.Thumb!.Path.Should().Be(validImagePath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    public void UpdateThumbHalf()
    {
        var video = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        video.UpdateThumbHalf(validImagePath);

        video.ThumbHalf.Should().NotBeNull();
        video.ThumbHalf!.Path.Should().Be(validImagePath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateBanner))]
    public void UpdateBanner()
    {
        var video = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        video.UpdateBanner(validImagePath);

        video.Banner.Should().NotBeNull();
        video.Banner!.Path.Should().Be(validImagePath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateMedia))]
    public void UpdateMedia()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();

        video.UpdateMedia(validMediaPath);

        video.Media.Should().NotBeNull();
        video.Media!.FilePath.Should().Be(validMediaPath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateTrailer))]
    public void UpdateTrailer()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();

        video.UpdateTrailer(validMediaPath);

        video.Trailer.Should().NotBeNull();
        video.Trailer!.FilePath.Should().Be(validMediaPath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    public void UpdateAsSentToEncode()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();
        video.UpdateMedia(validMediaPath);

        video.UpdateAsSentToEncode();

        video.Media.Should().NotBeNull();
        video.Media!.Status.Should().Be(MediaStatus.Processing);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
    public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
    {
        var video = _fixture.GetValidVideo();

        var action = () => video.UpdateAsSentToEncode();

        action
            .Should()
            .ThrowExactly<EntityValidationException>()
            .WithMessage("Media should not be null.");
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    public void UpdateAsEncoded()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();
        video.UpdateMedia(validMediaPath);
        var validEncodedPath = _fixture.GetValidMediaPath();

        video.UpdateAsEncoded(validEncodedPath);

        video.Media.Should().NotBeNull();
        video.Media!.EncodedPath.Should().Be(validEncodedPath);
        video.Media.Status.Should().Be(MediaStatus.Completed);
    }


    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsEncodedThrowsThereIsNoMedia))]
    public void UpdateAsEncodedThrowsThereIsNoMedia()
    {
        var video = _fixture.GetValidVideo();
        var validEncodedPath = _fixture.GetValidMediaPath();

        var action = () =>video.UpdateAsEncoded(validEncodedPath);

        action
            .Should()
            .ThrowExactly<EntityValidationException>()
            .WithMessage("Media should not be null.");
    }
}
