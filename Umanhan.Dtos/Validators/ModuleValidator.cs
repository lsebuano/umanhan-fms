using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class ModuleValidator : AbstractValidator<ModuleDto>
    {
        public ModuleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");
        }
    }
}
