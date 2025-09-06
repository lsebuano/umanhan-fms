using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmSetupValidator : AbstractValidator<FarmSetupDto>
    {
        public FarmSetupValidator()
        {
            RuleFor(x => x.FarmName)
                .NotEmpty().WithMessage("Farm Name is required.")
                .MaximumLength(255).WithMessage("Farm Name cannot exceed 255 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");

            RuleFor(x => x.SizeInHectares)
                .NotEmpty().WithMessage("Farm size is required.")
                .GreaterThan(0).WithMessage("Farm size must be greater than zero.");

            RuleFor(x => x.OwnerName)
                .NotEmpty().WithMessage("Owner name is required.");
        }
    }
}
