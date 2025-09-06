using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer Name is required.")
                .MaximumLength(255).WithMessage("Customer Name cannot exceed 255 characters.");

            RuleFor(x => x.CustomerTypeId)
                .NotEmpty().WithMessage("Customer Type is required.");
        }
    }
}
