using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmContractValidator : AbstractValidator<FarmContractDto>
    {
        public FarmContractValidator()
        {
            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.ContractDate)
                .NotEmpty().WithMessage("Contract Date is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .When(x => x.ContractId != Guid.Empty);

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer is required.");
        }
    }
}
