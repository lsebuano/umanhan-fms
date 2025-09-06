using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            //RuleFor(x => x.PhoneNumber)
            //    .NotEmpty().WithMessage("Phone Number is required.")
            //    .MaximumLength(15).WithMessage("Phone Number cannot exceed 15 characters.");
        }
    }
}
