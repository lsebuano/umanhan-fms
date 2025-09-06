using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmActivityPhotoEndpoints
    {
        private readonly FarmActivityService _farmActivityService;
        private readonly FarmActivityPhotoService _farmActivityPhotoService;
        private readonly IValidator<FarmActivityPhotoDto> _validator;
        private readonly ILogger<FarmActivityPhotoEndpoints> _logger;

        private const string THUMBS_FOLDER = "thumbnails/";

        public FarmActivityPhotoEndpoints(FarmActivityPhotoService farmActivityPhotoService,
            IValidator<FarmActivityPhotoDto> validator,
            ILogger<FarmActivityPhotoEndpoints> logger,
            FarmActivityService farmActivityService)
        {
            _farmActivityPhotoService = farmActivityPhotoService;
            _validator = validator;
            _logger = logger;
            _farmActivityService = farmActivityService;
        }

        public async Task<IResult> GetFarmActivityPhotoByActivityAsync(Guid activityId)
        {
            try
            {
                var farmActivityPhotos = await _farmActivityPhotoService.GetFarmActivityPhotosByActivityAsync(activityId).ConfigureAwait(false);
                return farmActivityPhotos is not null ? Results.Ok(farmActivityPhotos) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity photos by activity ID {ActivityId}", activityId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmActivityPhotoByIdAsync(Guid id)
        {
            try
            {
                var farmActivityPhoto = await _farmActivityPhotoService.GetFarmActivityPhotoByIdAsync(id).ConfigureAwait(false);
                return farmActivityPhoto is not null ? Results.Ok(farmActivityPhoto) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity photo by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmActivityPhotoAsync(FarmActivityPhotoDto farmActivityPhoto)
        {
            var validationResult = await _validator.ValidateAsync(farmActivityPhoto).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmActivityPhoto = await _farmActivityPhotoService.CreateFarmActivityPhotoAsync(farmActivityPhoto).ConfigureAwait(false);
                return Results.Created($"/api/farm-activity-photos/{newFarmActivityPhoto.PhotoId}", newFarmActivityPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm activity photo");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmActivityPhotoAsync(Guid id, FarmActivityPhotoDto farmActivityPhoto)
        {
            if (id != farmActivityPhoto.PhotoId)
            {
                return Results.BadRequest("Farm Activity Photo ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmActivityPhoto).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmActivityPhoto = await _farmActivityPhotoService.UpdateFarmActivityPhotoAsync(farmActivityPhoto).ConfigureAwait(false);
                return updatedFarmActivityPhoto is not null ? Results.Ok(updatedFarmActivityPhoto) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm activity photo with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmActivityPhotoAsync(Guid id)
        {
            try
            {
                var deletedFarmActivityPhoto = await _farmActivityPhotoService.DeleteFarmActivityPhotoAsync(id).ConfigureAwait(false);
                return deletedFarmActivityPhoto is not null ? Results.Ok(deletedFarmActivityPhoto) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm activity photo with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmActivityPhotoAsync(S3PhotoUploadDto obj, string s3BucketUrl)
        {
            if (string.IsNullOrEmpty(obj.S3ObjectKey))
            {
                return Results.BadRequest("S3 object key must be provided.");
            }

            try
            {
                var farmActivity = await _farmActivityService.GetFarmActivityByIdAsync(obj.ActivityId).ConfigureAwait(false);
                if (farmActivity is null)
                {
                    return Results.NotFound($"Farm Activity with ID {obj.ActivityId} not found.");
                }

                var farmActivityPhoto = new FarmActivityPhotoDto
                {
                    ActivityId = obj.ActivityId,
                    Notes = obj.Notes,
                    MimeType = Uri.UnescapeDataString(obj.S3ObjectContentType),
                    //ImageFull = obj.S3ObjectKey,
                    PhotoUrlFull = $"{s3BucketUrl.TrimEnd('/')}/{obj.S3ObjectKey.TrimStart('/')}",
                    //ImageThumbnail = $"{THUMBS_FOLDER}{obj.S3ObjectKey.TrimStart('/')}",
                    PhotoUrlThumbnail = $"{s3BucketUrl.TrimEnd('/')}/{THUMBS_FOLDER}{obj.S3ObjectKey.TrimStart('/')}"
                };

                var newFarmActivityPhoto = await _farmActivityPhotoService.CreateFarmActivityPhotoAsync(farmActivityPhoto).ConfigureAwait(false);
                return Results.Created($"/api/farm-activity-photos/{newFarmActivityPhoto.PhotoId}", newFarmActivityPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm activity photo with Activity ID {Id}", obj.ActivityId);
                return Results.Problem(ex.Message);
            }
        }
    }
}
