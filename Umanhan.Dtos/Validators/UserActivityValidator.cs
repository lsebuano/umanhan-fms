using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class UserActivityValidator : AbstractValidator<UserActivityDto>
    {
        public UserActivityValidator()
        {
            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.Timestamp)
                .NotEmpty().WithMessage("Timestamp is required.");
        }
    }
}
