using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class LivestockValidator : AbstractValidator<LivestockDto>
    {
        public LivestockValidator()
        {
            RuleFor(x => x.AnimalType)
                .NotEmpty().WithMessage("Animal Type is required.")
                .MaximumLength(255).WithMessage("Animal Type cannot exceed 255 characters.");
        }
    }
}
