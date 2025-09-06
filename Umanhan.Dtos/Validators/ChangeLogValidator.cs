using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class ChangeLogValidator : AbstractValidator<ChangeLogDto>
    {
        public ChangeLogValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Timestamp)
                .NotEmpty().WithMessage("Timestamp is required.");

            RuleFor(x => x.Entity)
                .NotEmpty().WithMessage("Entity is required.");

            RuleFor(x => x.Field)
                .NotEmpty().WithMessage("Field is required.");
        }
    }
}
