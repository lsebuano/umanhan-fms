using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class ProductTypeValidator : AbstractValidator<ProductTypeDto>
    {
        public ProductTypeValidator()
        {
            RuleFor(x => x.ProductTypeName)
                .NotEmpty().WithMessage("Product Type Name is required.")
                .MaximumLength(255).WithMessage("Product Type Name cannot exceed 255 characters.");
        }
    }
}
