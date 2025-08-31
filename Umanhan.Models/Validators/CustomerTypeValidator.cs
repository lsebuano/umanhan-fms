using FluentValidation;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Validators
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
