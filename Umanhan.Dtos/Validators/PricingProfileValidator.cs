using FluentValidation;

namespace Umanhan.Dtos.Validators
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
