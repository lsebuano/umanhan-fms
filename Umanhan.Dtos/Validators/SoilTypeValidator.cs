using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class SoilTypeValidator : AbstractValidator<SoilTypeDto>
    {
        public SoilTypeValidator()
        {
            RuleFor(x => x.SoilName)
                .NotEmpty().WithMessage("Soil Name is required.")
                .MaximumLength(255).WithMessage("Soil Name cannot exceed 255 characters.");
        }
    }
}
