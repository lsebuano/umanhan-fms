using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class InventoryValidator : AbstractValidator<InventoryDto>
    {
        public InventoryValidator()
        {
            RuleFor(x => x.ItemName)
                .NotEmpty().WithMessage("Item Name is required.")
                .MaximumLength(255).WithMessage("Item Name cannot exceed 255 characters.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");
        }
    }
}
