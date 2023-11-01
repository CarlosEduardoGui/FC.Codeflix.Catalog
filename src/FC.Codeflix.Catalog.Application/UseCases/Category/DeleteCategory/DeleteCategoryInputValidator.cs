using FluentValidation;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
public class DeleteCategoryInputValidator : AbstractValidator<DeleteCategoryInput>
{
    public DeleteCategoryInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(string.Format(ConstantsMessages.FieldNotEmpty, "Id"));
    }
}
