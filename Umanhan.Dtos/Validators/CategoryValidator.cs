using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category Name is required.")
                .MaximumLength(255).WithMessage("Category Name cannot exceed 255 characters.");
        }
    }
}
