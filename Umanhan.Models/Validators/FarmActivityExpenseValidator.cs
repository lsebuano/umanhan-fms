using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmActivityExpenseValidator : AbstractValidator<FarmActivityExpenseDto>
    {
        public FarmActivityExpenseValidator()
        {
            RuleFor(x => x.ActivityId)
                .NotEmpty().WithMessage("Activity is required.");

            RuleFor(x => x.ExpenseTypeId)
                .NotEmpty().WithMessage("Type is required.");

            RuleFor(x => x.DateUtc)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
