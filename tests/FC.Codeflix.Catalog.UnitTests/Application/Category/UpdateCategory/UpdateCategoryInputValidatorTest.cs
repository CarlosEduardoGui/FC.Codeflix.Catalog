using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "UpdateCategoryInputValidator - Use Case")]
    [Fact(DisplayName = nameof(DontValidateWhenEmptyGuidId))]
    public void DontValidateWhenEmptyGuidId()
    {
        var input = _fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        var validatResult = validator.Validate(input);

        validatResult.Should().NotBeNull();
        validatResult.IsValid.Should().BeFalse();
        validatResult.Errors.Should().HaveCount(1);
        validatResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }

    [Trait("Use Cases", "UpdateCategoryInputValidator - Use Case")]
    [Fact(DisplayName = nameof(ValidateOk))]
    public void ValidateOk()
    {
        var input = _fixture.GetValidInput();
        var validator = new UpdateCategoryInputValidator();

        var validatResult = validator.Validate(input);

        validatResult.Should().NotBeNull();
        validatResult.IsValid.Should().BeTrue();
        validatResult.Errors.Should().HaveCount(0);
    }
}
