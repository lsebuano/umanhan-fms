using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class CropValidator : AbstractValidator<CropDto>
    {
        public CropValidator()
        {
            RuleFor(x => x.CropName)
                .NotEmpty().WithMessage("Crop Name is required.")
                .MaximumLength(255).WithMessage("Crop Name cannot exceed 255 characters.");

            RuleFor(x => x.DefaultUnitId)
                .NotEmpty().WithMessage("Default Unit is required.");

            RuleFor(x => x.DefaultRatePerUnit)
                .NotEmpty().WithMessage("Default Rate per Unit is required.")
                .GreaterThan(0).WithMessage("Default rate per Unit must be greater than zero.");
        }
    }
}
