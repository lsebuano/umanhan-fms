using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
{
    public class FarmCropValidator : AbstractValidator<FarmCropDto>
    {
        public FarmCropValidator()
        {
            RuleFor(x => x.CropId)
                .NotEmpty().WithMessage("Crop is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.");

            RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

            RuleFor(x => x.ZoneId)
                .NotEmpty().WithMessage("Zone is required.");

            RuleFor(x => x.DefaultRate)
                .GreaterThan(0).WithMessage("Default Rate per Unit is required.");

            RuleFor(x => x.PlantingDateUtc)
                .NotEmpty().WithMessage("Planting Date is required.");
        }
    }
}
