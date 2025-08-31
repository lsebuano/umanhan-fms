using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
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
