using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmProduceInventoryValidator : AbstractValidator<FarmProduceInventoryDto>
    {
        public FarmProduceInventoryValidator()
        {
            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.ProductTypeId)
                .NotEmpty().WithMessage("Product Type is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.");


        }
    }
}
