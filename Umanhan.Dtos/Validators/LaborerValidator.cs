using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class LaborerValidator : AbstractValidator<LaborerDto>
    {
        public LaborerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

            RuleFor(x => x.DailyRate)
                .GreaterThan(0).WithMessage("Daily Rate must be greater than zero.");

            RuleFor(x => x.ContractedRate)
                .GreaterThan(0).WithMessage("Contracted Rate must be greater than zero.");
        }
    }
}
