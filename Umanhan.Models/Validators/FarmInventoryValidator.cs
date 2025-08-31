using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmInventoryValidator : AbstractValidator<FarmInventoryDto>
    {
        public FarmInventoryValidator()
        {
            RuleFor(x => x.InventoryId)
                .NotEmpty().WithMessage("Inventory is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.");

            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
