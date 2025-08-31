using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class RolePermissionValidator : AbstractValidator<RolePermissionDto>
    {
        public RolePermissionValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role is required.");

            RuleFor(x => x.ModuleId)
                .NotEmpty().WithMessage("Module is required.");

            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("Permission is required.");
        }
    }
}
