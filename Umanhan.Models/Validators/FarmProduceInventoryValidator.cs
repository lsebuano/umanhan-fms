using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
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
