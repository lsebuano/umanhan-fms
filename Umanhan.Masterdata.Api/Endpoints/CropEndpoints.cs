using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class CropEndpoints
    {
        private readonly CropService _cropService;
        private readonly IValidator<CropDto> _validator;
        private readonly ILogger<CropEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        //private const string MODULE_CACHE_KEY = "crop";

        public CropEndpoints(CropService cropService, 
            IValidator<CropDto> validator, 
            ILogger<CropEndpoints> logger
            )
        {
            _cropService = cropService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllCropsAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                    var result = await _cropService.GetAllCropsAsync("DefaultUnit").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving crops");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetCropByIdAsync(Guid id)
        {
            try
            {
                var crop = await _cropService.GetCropByIdAsync(id, "DefaultUnit").ConfigureAwait(false);
                return crop is not null ? Results.Ok(crop) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving crop with ID {CropId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateCropAsync(CropDto crop)
        {
            var validationResult = await _validator.ValidateAsync(crop).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newCrop = await _cropService.CreateCropAsync(crop).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/crops/{newCrop.CropId}", newCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating crop");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateCropAsync(Guid id, CropDto crop)
        {
            if (id != crop.CropId)
                return Results.BadRequest("Crop ID mismatch");

            var validationResult = await _validator.ValidateAsync(crop).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedCrop = await _cropService.UpdateCropAsync(crop).ConfigureAwait(false);
                if (updatedCrop is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedCrop);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating crop with ID {CropId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteCropAsync(Guid id)
        {
            try
            {
                var deletedCrop = await _cropService.DeleteCropAsync(id).ConfigureAwait(false);
                if (deletedCrop is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedCrop);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting crop with ID {CropId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
