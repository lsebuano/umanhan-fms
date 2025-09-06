using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class StaffValidator : AbstractValidator<StaffDto>
    {
        public StaffValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.HireDateUtc)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Hire Date must not be in the future.");
        }
    }
}
