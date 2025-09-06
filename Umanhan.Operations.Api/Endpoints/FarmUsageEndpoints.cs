using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmUsageEndpoints
    {
        private readonly FarmActivityUsageService _farmUsageService;
        private readonly IValidator<FarmActivityUsageDto> _validator;
        private readonly ILogger<FarmUsageEndpoints> _logger;

        public FarmUsageEndpoints(FarmActivityUsageService farmUsageService, IValidator<FarmActivityUsageDto> validator, ILogger<FarmUsageEndpoints> logger)
        {
            _farmUsageService = farmUsageService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmActivityUsagesAsync(Guid farmId)
        {
            try
            {
                var farmUsages = await _farmUsageService.GetFarmActivityUsagesAsync(farmId, "Inventory.Unit").ConfigureAwait(false);
                return Results.Ok(farmUsages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity usages for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmActivityUsagesByItemAsync(Guid farmId, Guid itemId)
        {
            try
            {
                var farmUsages = await _farmUsageService.GetFarmActivityUsagesByItemAsync(farmId, itemId, "Activity").ConfigureAwait(false);
                return Results.Ok(farmUsages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity usages for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmUsageByIdAsync(Guid id)
        {
            try
            {
                var farmUsage = await _farmUsageService.GetFarmActivityUsageByIdAsync(id, "Inventory.Unit").ConfigureAwait(false);
                return farmUsage is not null ? Results.Ok(farmUsage) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity usage by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmUsageByActivityAsync(Guid activityId)
        {
            try
            {
                var farmUsage = await _farmUsageService.GetFarmActivityUsagesByActivityAsync(activityId, "Inventory.Unit").ConfigureAwait(false);
                return farmUsage is not null ? Results.Ok(farmUsage) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity usages by activity ID {ActivityId}", activityId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmUsageAsync(FarmActivityUsageDto farmUsage)
        {
            var validationResult = await _validator.ValidateAsync(farmUsage).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmUsage = await _farmUsageService.CreateFarmActivityUsageAsync(farmUsage).ConfigureAwait(false);
                return Results.Created($"/api/farm-usages/{newFarmUsage.UsageId}", newFarmUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm activity usage");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmUsageAsync(Guid id, FarmActivityUsageDto farmUsage)
        {
            if (id != farmUsage.UsageId)
            {
                return Results.BadRequest("Farm Usage ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmUsage).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmUsage = await _farmUsageService.UpdateFarmActivityUsageAsync(farmUsage).ConfigureAwait(false);
                return updatedFarmUsage is not null ? Results.Ok(updatedFarmUsage) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm activity usage with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmUsageAsync(Guid id)
        {
            try
            {
                var deletedFarmUsage = await _farmUsageService.DeleteFarmActivityUsageAsync(id).ConfigureAwait(false);
                return deletedFarmUsage is not null ? Results.Ok(deletedFarmUsage) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm activity usage with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
