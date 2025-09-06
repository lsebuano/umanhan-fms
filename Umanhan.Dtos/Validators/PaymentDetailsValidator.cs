using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class PaymentDetailsValidator : AbstractValidator<PaymentDetailsDto>
    {
        public PaymentDetailsValidator()
        {
            RuleFor(x => x.BuyerName)
                .NotEmpty().WithMessage("Buyer name is required.");
            RuleFor(x => x.BuyerTin)
                .NotEmpty().WithMessage("TIN is required.");
            RuleFor(x => x.BuyerContactNo)
                .NotEmpty().WithMessage("Contact number is required.");
            RuleFor(x => x.BuyerAddress)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment Method is required.");
            RuleFor(x => x.PaymentRef)
                .NotEmpty().WithMessage("Payment reference number is required.");

            RuleFor(x => x.ActualPaidAmount)
                .GreaterThan(0).WithMessage("Payment must be greater than 0.");
        }
    }
}
