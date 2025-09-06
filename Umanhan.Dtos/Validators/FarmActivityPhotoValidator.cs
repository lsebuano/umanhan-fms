using FluentValidation;

namespace Umanhan.Dtos.Validators
{
    public class FarmActivityPhotoValidator : AbstractValidator<FarmActivityPhotoDto>
    {
        public FarmActivityPhotoValidator()
        {
            RuleFor(x => x.ActivityId)
                .NotEmpty().WithMessage("Activity is required.");

            RuleFor(x => x.PhotoUrlThumbnail)
                .NotEmpty().WithMessage("Photo Url Thumbnail is required.");

            RuleFor(x => x.PhotoUrlFull)
                .NotEmpty().WithMessage("Photo Url Full is required.");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("Mime Type is required.");
        }
    }
}
