using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class UnitValidator : AbstractValidator<UnitDto>
    {
        public UnitValidator()
        {
            RuleFor(x => x.UnitName)
                .NotEmpty().WithMessage("Unit Name is required.")
                .MaximumLength(255).WithMessage("Unit Name cannot exceed 255 characters.");
        }
    }
}
