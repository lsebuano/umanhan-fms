using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmGeneralExpenseReceiptValidator : AbstractValidator<FarmGeneralExpenseReceiptDto>
    {
        public FarmGeneralExpenseReceiptValidator()
        {
            RuleFor(x => x.GeneralExpenseId)
                .NotEmpty().WithMessage("General Expense is required.");

            RuleFor(x => x.ReceiptUrlThumbnail)
                .NotEmpty().WithMessage("Photo Url Thumbnail is required.");

            RuleFor(x => x.ReceiptUrlFull)
                .NotEmpty().WithMessage("Photo Url Full is required.");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("Mime Type is required.");
        }
    }
}
