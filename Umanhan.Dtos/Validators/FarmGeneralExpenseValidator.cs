using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmGeneralExpenseValidator : AbstractValidator<FarmGeneralExpenseDto>
    {
        public FarmGeneralExpenseValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.");

            //RuleFor(x => x.ExpenseTypeId)
            //    .NotEmpty().WithMessage("Expense Type is required.")
            //    .When(x => x.ExpenseTypeId != Guid.Empty);

            RuleFor(x => x.ExpenseTypeId)
                .NotEmpty().WithMessage("Expense Type is required.");

            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.")
                .When(x => x.FarmId != Guid.Empty);
        }
    }
}
