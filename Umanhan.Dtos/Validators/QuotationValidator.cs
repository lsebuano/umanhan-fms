using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class QuotationValidator : AbstractValidator<QuotationDto>
    {
        public QuotationValidator()
        {
            //RuleFor(x => x.ProfileId)
            //    .NotEmpty().WithMessage("Pricing Profile is required.");

            //RuleFor(x => x.ConditionTypeId)
            //    .NotEmpty().WithMessage("Condition Type is required.");

            //RuleFor(x => x.ApplyType)
            //    .NotEmpty().WithMessage("Apply Type is required.");

            //RuleFor(x => x.Value)
            //    .NotEmpty().WithMessage("Value is required.");

            //RuleFor(x => x.Sequence)
            //    .NotEmpty().WithMessage("Sequence is required.")
            //    .GreaterThan(0).WithMessage("Sequence must be greated than 0.");
        }
    }
}
