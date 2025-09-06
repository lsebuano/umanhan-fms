using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class LogValidator : AbstractValidator<LogDto>
    {
        public LogValidator()
        {
            RuleFor(x => x.Timestamp)
                .NotEmpty().WithMessage("Timestamp is required.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.");

            RuleFor(x => x.Level)
                .NotEmpty().WithMessage("Level is required.");
        }
    }
}
