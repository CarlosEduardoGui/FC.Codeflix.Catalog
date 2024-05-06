using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoValidatorTest
{
    private readonly VideoTestFixture _fixture;

    public VideoValidatorTest(VideoTestFixture videoTestFixture)
        => _fixture = videoTestFixture;

    [Trait("Domain", "Video Validator - Aggregate")]
    [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
    public void ReturnsValidWhenVideoIsValid()
    {
        var validVideo = _fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeFalse();
        notificationValidationHandler.Errors.Should().BeEmpty();
    }

    [Trait("Domain", "Video Validator - Aggregate")]
    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsTooLong))]
    public void ReturnsErrorWhenTitleIsTooLong()
    {
        var validVideo = new DomainEntity.Video(
            _fixture.GetTooLongTitle(),
            _fixture.GetValidDescription(),
            _fixture.GetValidYearLauched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler
            .Errors
            .ToList()
            .First()
            .Message.Should().Be("Title should be less or equal 255 characters long.");
    }

    [Trait("Domain", "Video Validator - Aggregate")]
    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleEmpty))]
    public void ReturnsErrorWhenTitleEmpty()
    {
        var validVideo = new DomainEntity.Video(
            string.Empty,
            _fixture.GetValidDescription(),
            _fixture.GetValidYearLauched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler
            .Errors
            .ToList()
            .First()
            .Message.Should().Be("Title should not be empty.");
    }

    [Trait("Domain", "Video Validator - Aggregate")]
    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionEmpty))]
    public void ReturnsErrorWhenDescriptionEmpty()
    {
        var validVideo = new DomainEntity.Video(
            _fixture.GetValidTitle(),
            string.Empty,
            _fixture.GetValidYearLauched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler
            .Errors
            .ToList()
            .First()
            .Message.Should().Be("Description should not be empty.");
    }

    [Trait("Domain", "Video Validator - Aggregate")]
    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionTooLong))]
    public void ReturnsErrorWhenDescriptionTooLong()
    {
        var validVideo = new DomainEntity.Video(
            _fixture.GetValidTitle(),
            _fixture.GetTooLongDescription(),
            _fixture.GetValidYearLauched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler
            .Errors
            .ToList()
            .First()
            .Message.Should().Be("Description should be less or equal 4000 characters long.");
    }
}
