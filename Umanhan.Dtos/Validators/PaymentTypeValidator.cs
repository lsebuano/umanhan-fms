using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class PaymentTypeValidator : AbstractValidator<PaymentTypeDto>
    {
        public PaymentTypeValidator()
        {
            RuleFor(x => x.PaymentTypeName)
                .NotEmpty().WithMessage("Payment Type Name is required.")
                .MaximumLength(255).WithMessage("Payment Type Name cannot exceed 255 characters.");
        }
    }
}
