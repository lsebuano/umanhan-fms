using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmActivityEndpoints
    {
        private readonly FarmActivityService _farmActivityService;
        private readonly IValidator<FarmActivityDto> _validator;
        private readonly ILogger<FarmActivityEndpoints> _logger;

        public FarmActivityEndpoints(FarmActivityService farmActivityService, IValidator<FarmActivityDto> validator, ILogger<FarmActivityEndpoints> logger)
        {
            _farmActivityService = farmActivityService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmActivitiesAsync(Guid farmId)
        {
            try
            {
                var farmActivities = await _farmActivityService.GetFarmActivitiesAsync(farmId).ConfigureAwait(false);
                return Results.Ok(farmActivities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activities for farm ID {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmActivitiesAsync(Guid farmId, DateTime date)
        {
            try
            {
                var farmActivities = await _farmActivityService.GetFarmActivitiesAsync(farmId, date, "Farm", "ProductType", "Supervisor", "Task").ConfigureAwait(false);
                return Results.Ok(farmActivities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activities for farm ID {FarmId} on date {Date}", farmId, date);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmActivityByIdAsync(Guid id)
        {
            try
            {
                var farmActivity = await _farmActivityService.GetFarmActivityByIdAsync(id, "Farm", "Task").ConfigureAwait(false);
                return farmActivity is not null ? Results.Ok(farmActivity) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmActivityExpensesAsync(Guid activityId)
        {
            try
            {
                var farmActivityExpenses = await _farmActivityService.GetFarmActivityExpensesAsync(activityId, "Expense", "Activity.Task", "Activity.ProductType", "Activity.Supervisor", "Activity.Contract.Customer").ConfigureAwait(false);
                return Results.Ok(farmActivityExpenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity expenses for activity ID {ActivityId}", activityId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmActivityAsync(FarmActivityDto farmActivity)
        {
            var validationResult = await _validator.ValidateAsync(farmActivity).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmActivity = await _farmActivityService.CreateFarmActivityAsync(farmActivity).ConfigureAwait(false);
                return Results.Created($"/api/farm-activities/{newFarmActivity.ActivityId}", newFarmActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm activity");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmActivityAsync(Guid id, FarmActivityDto farmActivity)
        {
            if (id != farmActivity.ActivityId)
            {
                return Results.BadRequest("Farm Activity ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmActivity).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmActivity = await _farmActivityService.UpdateFarmActivityAsync(farmActivity).ConfigureAwait(false);
                return updatedFarmActivity is not null ? Results.Ok(updatedFarmActivity) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm activity with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmActivityAsync(Guid id)
        {
            try
            {
                var deletedFarmActivity = await _farmActivityService.DeleteFarmActivityAsync(id).ConfigureAwait(false);
                return deletedFarmActivity is not null ? Results.Ok(deletedFarmActivity) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm activity with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
