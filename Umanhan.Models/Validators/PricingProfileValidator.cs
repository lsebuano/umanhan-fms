using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class PricingProfileValidator : AbstractValidator<PricingProfileDto>
    {
        public PricingProfileValidator()
        {
            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}
