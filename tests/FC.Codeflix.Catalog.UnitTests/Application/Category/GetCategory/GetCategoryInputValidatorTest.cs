using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryInputValidatorTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryInputValidatorTest(GetCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "GetCategoryInputValition - UseCases")]
    [Fact(DisplayName = nameof(ValidationOk))]
    public void ValidationOk()
    {
        var validInput = new GetCategoryInput(Guid.NewGuid());
        var validator = new GetCategoryInputValidator();

        var validationResult = validator.Validate(validInput);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }

    [Trait("Use Cases", "GetCategoryInputValition - UseCases")]
    [Fact(DisplayName = nameof(InvalidWhenEmptyGuidId))]
    public void InvalidWhenEmptyGuidId()
    {
        var invalidInput = new GetCategoryInput(Guid.Empty);
        var validator = new GetCategoryInputValidator();

        var validationResult = validator.Validate(invalidInput);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }
}
