using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmTransactionValidator : AbstractValidator<FarmTransactionDto>
    {
        public FarmTransactionValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.");

            //RuleFor(x => x.ProduceInventoryId)
            //    .NotEmpty().WithMessage("Produce Inventory is required.");

            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.TransactionTypeId)
                .NotEmpty().WithMessage("Transaction Type is required.");

            RuleFor(x => x.ProductTypeId)
                .NotEmpty().WithMessage("Product Type is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit Price is required.")
                .When(x => x.TransactionType != null && 
                           x.TransactionType.Equals(Dtos.HelperModels.TransactionType.SALE.ToString(), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity is required.");

            RuleFor(x => x.BuyerName)
                .NotEmpty().WithMessage("Name is required.")
                .When(x => x.BuyerId != null && x.BuyerId != Guid.Empty);

            RuleFor(x => x.BuyerId)
                .NotEmpty().WithMessage("Name is required for Donation transactions.")
                .When(x => x.TransactionType != null &&
                           x.TransactionType.Equals(Dtos.HelperModels.TransactionType.DONATION.ToString(), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.BuyerName)
                .NotEmpty().WithMessage("Name is required for Donation transactions.")
                .When(x => x.TransactionType != null &&
                           x.TransactionType.Equals(Dtos.HelperModels.TransactionType.DONATION.ToString(), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.Notes)
                .MaximumLength(100).WithMessage("Notes should not be more than 100 characters.");
        }
    }
}
