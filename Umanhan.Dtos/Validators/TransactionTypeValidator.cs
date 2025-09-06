using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class TransactionTypeValidator : AbstractValidator<TransactionTypeDto>
    {
        public TransactionTypeValidator()
        {
            RuleFor(x => x.TransactionTypeName)
                .NotEmpty().WithMessage("Transaction Type Name is required.")
                .MaximumLength(255).WithMessage("Transaction Type Name cannot exceed 255 characters.");
        }
    }
}
