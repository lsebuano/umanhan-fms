using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmContractSaleValidator : AbstractValidator<FarmContractSaleDto>
    {
        public FarmContractSaleValidator()
        {
            RuleFor(x => x.ContractDetailId)
                .NotEmpty().WithMessage("Contract Detail is required.")
                .When(x => x.ContractDetailId != Guid.Empty);

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product is required.")
                .When(x => x.ProductId != Guid.Empty);

            RuleFor(x => x.ProductTypeId)
                .NotEmpty().WithMessage("Product Type is required.")
                .When(x => x.ProductTypeId != Guid.Empty);

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.")
                .When(x => x.UnitId != Guid.Empty);

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer is required.")
                .When(x => x.CustomerId != Guid.Empty);

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.")
                .When(x => x.ProductTypeId != Guid.Empty);

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity is required.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit Price is required.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total Amount is required.");
        }
    }
}
