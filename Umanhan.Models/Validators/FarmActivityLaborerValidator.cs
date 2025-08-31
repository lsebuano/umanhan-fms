using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmActivityLaborerValidator : AbstractValidator<FarmActivityLaborerDto>
    {
        public FarmActivityLaborerValidator()
        {
            RuleFor(x => x.ActivityId)
                .NotEmpty().WithMessage("Activity is required.");

            RuleFor(x => x.PaymentTypeId)
                .NotEmpty().WithMessage("Payment Type is required.");

            RuleFor(x => x.LaborerId)
                .NotEmpty().WithMessage("Labor is required.");

            //RuleFor(x => x.DateUtc)
            //    .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.Rate)
                .GreaterThan(0).WithMessage("Rate must be greater than zero.");

            RuleFor(x => x.QuantityWorked)
                .NotEmpty().WithMessage("Quantity Worked must be greater than zero.");

        }
    }
}
