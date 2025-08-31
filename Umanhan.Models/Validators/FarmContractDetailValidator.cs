using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmContractDetailValidator : AbstractValidator<FarmContractDetailDto>
    {
        public FarmContractDetailValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty().WithMessage("Contract is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.");

            RuleFor(x => x.ContractedUnitPrice)
                .GreaterThan(0).WithMessage("Unit Price is required.");

            RuleFor(x => x.ContractedQuantity)
                .GreaterThan(0).WithMessage("Quantity is required.");

            RuleFor(x => x.PickupDate)
                .GreaterThanOrEqualTo(x => x.HarvestDate)
                .WithMessage("Pickup Date must be the same or after the Harvest Date.");
        }
    }
}
