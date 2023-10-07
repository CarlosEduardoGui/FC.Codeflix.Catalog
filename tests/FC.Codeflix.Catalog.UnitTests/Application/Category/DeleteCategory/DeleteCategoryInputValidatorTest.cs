using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryInputValidatorTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryInputValidatorTest(DeleteCategoryTestFixture fixture) => _fixture = fixture;

    [Trait("Use Cases", "DeleteCategoryInputValition - Use Cases")]
    [Fact(DisplayName = nameof(ValidationOk))]
    public void ValidationOk()
    {
        var validInput = new DeleteCategoryInput(Guid.NewGuid());
        var deleteCategoryValidator = new DeleteCategoryInputValidator();

        var validation = deleteCategoryValidator.Validate(validInput);

        validation.Should().NotBeNull();
        validation.IsValid.Should().BeTrue();
        validation.Errors.Should().BeEmpty();
    }

    [Trait("Use Cases", "DeleteCategoryInputValition - Use Cases")]
    [Fact(DisplayName = nameof(InvalidWhenEmptyGuidId))]
    public void InvalidWhenEmptyGuidId()
    {
        var invalidInput = new DeleteCategoryInput(Guid.Empty);
        var deleteCategoryValidator = new DeleteCategoryInputValidator();

        var validation = deleteCategoryValidator.Validate(invalidInput);

        validation.Should().NotBeNull();
        validation.IsValid.Should().BeFalse();
        validation.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }
}
