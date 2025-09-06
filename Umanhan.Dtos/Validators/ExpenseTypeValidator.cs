using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class ExpenseTypeValidator : AbstractValidator<ExpenseTypeDto>
    {
        public ExpenseTypeValidator()
        {
            RuleFor(x => x.ExpenseTypeName)
                .NotEmpty().WithMessage("Expense Type Name is required.")
                .MaximumLength(255).WithMessage("Expense Type Name cannot exceed 255 characters.");
        }
    }
}
