using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class TaskValidator : AbstractValidator<TaskDto>
    {
        public TaskValidator()
        {
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("Task Name is required.")
                .MaximumLength(255).WithMessage("Task Name cannot exceed 255 characters.");
        }
    }
}
