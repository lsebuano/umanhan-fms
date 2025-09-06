using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmZoneValidator : AbstractValidator<FarmZoneDto>
    {
        public FarmZoneValidator()
        {
            RuleFor(x => x.ZoneName)
                .NotEmpty().WithMessage("Zone Name is required.");

            //RuleFor(x => x.FarmId)
            //    .NotEmpty().WithMessage("Farm is required.");

            //RuleFor(x => x.AreaInSqm)
            //    .GreaterThan(0).WithMessage("Zone area must be greater than zero.");

            RuleFor(x => x.SizeInHectares)
                .GreaterThan(0).WithMessage("Zone hectares must be greater than zero.");

            RuleFor(x => x.CropId)
                .NotEmpty().WithMessage("Crop is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required.");
        }
    }
}
