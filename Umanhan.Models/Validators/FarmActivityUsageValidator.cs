using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmActivityUsageValidator : AbstractValidator<FarmActivityUsageDto>
    {
        public FarmActivityUsageValidator()
        {
            RuleFor(x => x.ActivityId)
                .NotEmpty().WithMessage("Activity is required.");

            RuleFor(x => x.UsageHours)
                .GreaterThan(0).WithMessage("Usage Hours be greater than zero.");

            RuleFor(x => x.Rate)
                .GreaterThanOrEqualTo(0).WithMessage("Rate must be greater than or equal to zero.");

            RuleFor(x => x.InventoryId)
                .NotEmpty().WithMessage("Quantity Worked must be greater than zero.");

        }
    }
}
