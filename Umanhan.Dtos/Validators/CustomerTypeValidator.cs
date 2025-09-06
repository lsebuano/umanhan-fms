using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class CustomerTypeValidator : AbstractValidator<CustomerTypeDto>
    {
        public CustomerTypeValidator()
        {
            RuleFor(x => x.CustomerTypeName)
                .NotEmpty().WithMessage("Customer Type Name is required.")
                .MaximumLength(255).WithMessage("Customer Type Name cannot exceed 255 characters.");
        }
    }
}
