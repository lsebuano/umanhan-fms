using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class ExpenseTypeEndpoints
    {
        private readonly ExpenseTypeService _expenseTypeService;
        private readonly IValidator<ExpenseTypeDto> _validator;
        private readonly ILogger<ExpenseTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "expensetype";

        public ExpenseTypeEndpoints(ExpenseTypeService expenseTypeService, IValidator<ExpenseTypeDto> validator, ILogger<ExpenseTypeEndpoints> logger)
        {
            _expenseTypeService = expenseTypeService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllExpenseTypesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _expenseTypeService.GetAllExpenseTypesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetExpenseTypeByIdAsync(Guid id)
        {
            try
            {
                var expenseType = await _expenseTypeService.GetExpenseTypeByIdAsync(id).ConfigureAwait(false);
                return expenseType is not null ? Results.Ok(expenseType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense type with ID {ExpenseTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateExpenseTypeAsync(ExpenseTypeDto expenseType)
        {
            var validationResult = await _validator.ValidateAsync(expenseType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newExpenseType = await _expenseTypeService.CreateExpenseTypeAsync(expenseType).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/expense-types/{newExpenseType.TypeId}", newExpenseType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense type");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateExpenseTypeAsync(Guid id, ExpenseTypeDto expenseType)
        {
            if (id != expenseType.TypeId)
                return Results.BadRequest("Expense Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(expenseType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedExpenseType = await _expenseTypeService.UpdateExpenseTypeAsync(expenseType).ConfigureAwait(false);
                if (updatedExpenseType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedExpenseType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense type with ID {ExpenseTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteExpenseTypeAsync(Guid id)
        {
            try
            {
                var deletedExpenseType = await _expenseTypeService.DeleteExpenseTypeAsync(id).ConfigureAwait(false);
                if (deletedExpenseType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedExpenseType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense type with ID {ExpenseTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
