using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmLaborEndpoints
    {
        private readonly FarmActivityLaborerService _farmLaborService;
        private readonly IValidator<FarmActivityLaborerDto> _validator;
        private readonly ILogger<FarmLaborEndpoints> _logger;

        public FarmLaborEndpoints(FarmActivityLaborerService farmLaborService, IValidator<FarmActivityLaborerDto> validator, ILogger<FarmLaborEndpoints> logger)
        {
            _farmLaborService = farmLaborService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmActivityLaborersAsync(Guid farmId)
        {
            try
            {
                var farmLabors = await _farmLaborService.GetFarmActivityLaborersAsync(farmId, "Laborer", "PaymentType").ConfigureAwait(false);
                return Results.Ok(farmLabors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm laborers for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmLaborByIdAsync(Guid id)
        {
            try
            {
                var farmLabor = await _farmLaborService.GetFarmActivityLaborerByIdAsync(id, "Laborer", "PaymentType").ConfigureAwait(false);
                return farmLabor is not null ? Results.Ok(farmLabor) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm laborer by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmLaborByActivityAsync(Guid activityId)
        {
            try
            {
                var farmLabor = await _farmLaborService.GetFarmActivityLaborersByActivityAsync(activityId, "Laborer", "PaymentType").ConfigureAwait(false);
                return farmLabor is not null ? Results.Ok(farmLabor) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm laborers by activity ID {ActivityId}", activityId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmLaborAsync(FarmActivityLaborerDto farmLabor)
        {
            var validationResult = await _validator.ValidateAsync(farmLabor).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmLabor = await _farmLaborService.CreateFarmActivityLaborerAsync(farmLabor).ConfigureAwait(false);
                return Results.Created($"/api/farm-labors/{newFarmLabor.LaborActivityId}", newFarmLabor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm laborer");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmLaborAsync(Guid id, FarmActivityLaborerDto farmLabor)
        {
            if (id != farmLabor.LaborActivityId)
            {
                return Results.BadRequest("Farm Activity ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmLabor).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmLabor = await _farmLaborService.UpdateFarmActivityLaborerAsync(farmLabor).ConfigureAwait(false);
                return updatedFarmLabor is not null ? Results.Ok(updatedFarmLabor) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm laborer with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmLaborAsync(Guid id)
        {
            try
            {
                var deletedFarmLabor = await _farmLaborService.DeleteFarmActivityLaborerAsync(id).ConfigureAwait(false);
                return deletedFarmLabor is not null ? Results.Ok(deletedFarmLabor) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm laborer with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
