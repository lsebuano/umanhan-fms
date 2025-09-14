using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmActivityValidator : AbstractValidator<FarmActivityDto>
    {
        public FarmActivityValidator()
        {
            RuleSet("Create", () =>
            {
                RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

                RuleFor(x => x.TaskId)
                    .NotEmpty().WithMessage("Task is required.");

                RuleFor(x => x.SupervisorId)
                    .NotEmpty().WithMessage("Supervisor is required.");

                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Product is required.");

                RuleFor(x => x.ProductTypeId)
                    .NotEmpty().WithMessage("Type is required.");

                RuleFor(x => x.StartDateTime)
                    .NotEmpty().WithMessage("Start Date is required.");

                RuleFor(x => x.EndDateTime)
                    .NotEmpty().WithMessage("End Date is required.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(x => x.ActivityId)
                .NotEmpty().WithMessage("Activity Id is required.");

                RuleFor(x => x.FarmId)
                .NotEmpty().WithMessage("Farm is required.");

                RuleFor(x => x.TaskId)
                    .NotEmpty().WithMessage("Task is required.");

                RuleFor(x => x.SupervisorId)
                    .NotEmpty().WithMessage("Supervisor is required.");

                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Product is required.");

                RuleFor(x => x.ProductTypeId)
                    .NotEmpty().WithMessage("Type is required.");

                RuleFor(x => x.StartDateTime)
                    .NotEmpty().WithMessage("Start Date is required.");

                RuleFor(x => x.EndDateTime)
                    .NotEmpty().WithMessage("End Date is required.");
            });
        }
    }
}
