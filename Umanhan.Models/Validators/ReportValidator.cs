using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class ReportValidator : AbstractValidator<ReportDto>
    {
        public ReportValidator()
        {
            //RuleFor(x => x.TaskName)
            //    .NotEmpty().WithMessage("Task Name is required.")
            //    .MaximumLength(255).WithMessage("Task Name cannot exceed 255 characters.");
        }
    }
}
