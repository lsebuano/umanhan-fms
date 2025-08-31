using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmCropEndpoints
    {
        private readonly FarmCropService _farmCropService;
        private readonly IValidator<FarmCropDto> _validator;
        private readonly ILogger<FarmCropEndpoints> _logger;

        public FarmCropEndpoints(FarmCropService farmCropService, IValidator<FarmCropDto> validator, ILogger<FarmCropEndpoints> logger)
        {
            _farmCropService = farmCropService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetAllFarmCropsAsync()
        {
            try
            {
                var farmCrops = await _farmCropService.GetAllFarmCropsAsync("Farm", "Zone.Soil", "Crop.DefaultUnit").ConfigureAwait(false);
                return Results.Ok(farmCrops);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all farm crops");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmCropByIdAsync(Guid id)
        {
            try
            {
                var farmCrop = await _farmCropService.GetFarmCropByIdAsync(id, "Farm", "Zone.Soil", "Crop.DefaultUnit").ConfigureAwait(false);
                return farmCrop is not null ? Results.Ok(farmCrop) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm crop by ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmCropAsync(FarmCropDto farmCrop)
        {
            var validationResult = await _validator.ValidateAsync(farmCrop).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmCrop = await _farmCropService.CreateFarmCropAsync(farmCrop).ConfigureAwait(false);
                return Results.Created($"/api/farm-crops/{newFarmCrop.ZoneId}", newFarmCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm crop");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmCropAsync(Guid id, FarmCropDto farmCrop)
        {
            if (id != farmCrop.ZoneId)
            {
                return Results.BadRequest("Farm Crop ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmCrop).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmCrop = await _farmCropService.UpdateFarmCropAsync(farmCrop).ConfigureAwait(false);
                return updatedFarmCrop is not null ? Results.Ok(updatedFarmCrop) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm crop with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmCropAsync(Guid id)
        {
            try
            {
                var deletedFarmCrop = await _farmCropService.DeleteFarmCropAsync(id).ConfigureAwait(false);
                return deletedFarmCrop is not null ? Results.Ok(deletedFarmCrop) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm crop with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateUpdateFarmCropAsync(FarmCropDto farmCrop)
        {
            var validationResult = await _validator.ValidateAsync(farmCrop).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmCrop = await _farmCropService.CreateUpdateFarmCropAsync(farmCrop).ConfigureAwait(false);
                return Results.Created($"/api/farm-crops/{newFarmCrop.FarmCropId}", newFarmCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating or updating farm crop");
                return Results.Problem(ex.Message);
            }
        }
    }
}