using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmValidator : AbstractValidator<FarmDto>
    {
        public FarmValidator()
        {
            RuleFor(x => x.FarmName)
                .NotEmpty().WithMessage("Farm Name is required.")
                .MaximumLength(255).WithMessage("Farm Name cannot exceed 255 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");
        }
    }
}
