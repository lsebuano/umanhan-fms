using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmExpenseEndpoints
    {
        private readonly FarmActivityExpenseService _farmExpenseService;
        private readonly IValidator<FarmActivityExpenseDto> _validator;
        private readonly ILogger<FarmExpenseEndpoints> _logger;

        public FarmExpenseEndpoints(FarmActivityExpenseService farmExpenseService, IValidator<FarmActivityExpenseDto> validator, ILogger<FarmExpenseEndpoints> logger)
        {
            _farmExpenseService = farmExpenseService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmActivityExpensesAsync(Guid farmId)
        {
            try
            {
                var farmExpenses = await _farmExpenseService.GetFarmActivityExpensesAsync(farmId, "Activity.ProductType", "Activity.Supervisor", "Activity.Task", "ExpenseType").ConfigureAwait(false);
                return Results.Ok(farmExpenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity expenses for farm ID {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmExpenseByIdAsync(Guid id)
        {
            try
            {
                var farmExpense = await _farmExpenseService.GetFarmActivityExpenseByIdAsync(id, "Activity.ProductType", "Activity.Supervisor", "Activity.Task", "ExpenseType").ConfigureAwait(false);
                return farmExpense is not null ? Results.Ok(farmExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity expense by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmExpenseByActivityAsync(Guid activityId)
        {
            try
            {
                var farmExpense = await _farmExpenseService.GetFarmActivityExpensesByActivityAsync(activityId, "Activity.ProductType", "Activity.Supervisor", "Activity.Task", "ExpenseType").ConfigureAwait(false);
                return farmExpense is not null ? Results.Ok(farmExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm activity expenses by activity ID {ActivityId}", activityId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmExpenseAsync(FarmActivityExpenseDto farmExpense)
        {
            var validationResult = await _validator.ValidateAsync(farmExpense).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmExpense = await _farmExpenseService.CreateFarmActivityExpenseAsync(farmExpense).ConfigureAwait(false);
                return Results.Created($"/api/farm-expenses/{newFarmExpense.ExpenseId}", newFarmExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm activity expense");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmExpenseAsync(Guid id, FarmActivityExpenseDto farmExpense)
        {
            if (id != farmExpense.ExpenseId)
            {
                return Results.BadRequest("Farm Activity ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmExpense).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmExpense = await _farmExpenseService.UpdateFarmActivityExpenseAsync(farmExpense).ConfigureAwait(false);
                return updatedFarmExpense is not null ? Results.Ok(updatedFarmExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm activity expense with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmExpenseAsync(Guid id)
        {
            try
            {
                var deletedFarmExpense = await _farmExpenseService.DeleteFarmActivityExpenseAsync(id).ConfigureAwait(false);
                return deletedFarmExpense is not null ? Results.Ok(deletedFarmExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm activity expense with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
