using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmGeneralExpenseEndpoints
    {
        private readonly FarmGeneralExpenseService _farmGeneralExpenseService;
        private readonly IValidator<FarmGeneralExpenseDto> _validator;
        private readonly ILogger<FarmGeneralExpenseEndpoints> _logger;

        public FarmGeneralExpenseEndpoints(FarmGeneralExpenseService farmGeneralExpenseService, IValidator<FarmGeneralExpenseDto> validator, ILogger<FarmGeneralExpenseEndpoints> logger)
        {
            _farmGeneralExpenseService = farmGeneralExpenseService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmGeneralExpensesAsync(Guid farmId)
        {
            try
            {
                var farmGeneralExpenses = await _farmGeneralExpenseService.GetFarmGeneralExpensesAsync(farmId, "Farm", "ExpenseType").ConfigureAwait(false);
                return Results.Ok(farmGeneralExpenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmGeneralExpensesAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var farmGeneralExpenses = await _farmGeneralExpenseService.GetFarmGeneralExpensesAsync(farmId, startDate, endDate, "Farm", "ExpenseType").ConfigureAwait(false);
                return Results.Ok(farmGeneralExpenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmGeneralExpenseByIdAsync(Guid id)
        {
            try
            {
                var farmGeneralExpense = await _farmGeneralExpenseService.GetFarmGeneralExpenseByIdAsync(id, "Farm", "ExpenseType").ConfigureAwait(false);
                return farmGeneralExpense is not null ? Results.Ok(farmGeneralExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expense by ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmGeneralExpenseAsync(FarmGeneralExpenseDto farmGeneralExpense)
        {
            var validationResult = await _validator.ValidateAsync(farmGeneralExpense).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmGeneralExpense = await _farmGeneralExpenseService.CreateFarmGeneralExpenseAsync(farmGeneralExpense).ConfigureAwait(false);
                return Results.Created($"/api/farm-general-expenses/{newFarmGeneralExpense.ExpenseId}", newFarmGeneralExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm sale detail");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmGeneralExpenseAsync(Guid id, FarmGeneralExpenseDto farmGeneralExpense)
        {
            if (id != farmGeneralExpense.ExpenseId)
            {
                return Results.BadRequest("Farm General Expense ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmGeneralExpense).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmGeneralExpense = await _farmGeneralExpenseService.UpdateFarmGeneralExpenseAsync(farmGeneralExpense).ConfigureAwait(false);
                return updatedFarmGeneralExpense is not null ? Results.Ok(updatedFarmGeneralExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm sale with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmGeneralExpenseAsync(Guid id)
        {
            try
            {
                var deletedFarmGeneralExpense = await _farmGeneralExpenseService.DeleteFarmGeneralExpenseAsync(id).ConfigureAwait(false);
                return deletedFarmGeneralExpense is not null ? Results.Ok(deletedFarmGeneralExpense) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm sale with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
