using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Validators
{
    public class QuotationProductValidator : AbstractValidator<QuotationProductDto>
    {
        public QuotationProductValidator()
        {
            RuleFor(x => x.ProductTypeId)
                .NotEmpty().WithMessage("Type is required");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity is required");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit Price is required");
        }
    }
}
