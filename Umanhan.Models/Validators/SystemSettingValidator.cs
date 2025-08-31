using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Validators
{
    public class SystemSettingValidator : AbstractValidator<SystemSettingDto>
    {
        public SystemSettingValidator()
        {
            RuleFor(x => x.SettingName)
                .NotEmpty().WithMessage("Setting Name is required.")
                .MaximumLength(255).WithMessage("Setting Name cannot exceed 255 characters.");

            RuleFor(x => x.SettingValue)
                .NotEmpty().WithMessage("Setting Value is required.")
                .MaximumLength(255).WithMessage("Setting Value cannot exceed 255 characters.");
        }
    }
}
