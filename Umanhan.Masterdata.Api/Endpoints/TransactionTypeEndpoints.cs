using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class TransactionTypeEndpoints
    {
        private readonly TransactionTypeService _transactionTypeService;
        private readonly IValidator<TransactionTypeDto> _validator;
        private readonly ILogger<TransactionTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "transactiontype";

        public TransactionTypeEndpoints(TransactionTypeService transactionTypeService, IValidator<TransactionTypeDto> validator, ILogger<TransactionTypeEndpoints> logger)
        {
            _transactionTypeService = transactionTypeService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllTransactionTypesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _transactionTypeService.GetAllTransactionTypesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all transaction types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetTransactionTypeByIdAsync(Guid id)
        {
            try
            {
                var transactionType = await _transactionTypeService.GetTransactionTypeByIdAsync(id).ConfigureAwait(false);
                return transactionType is not null ? Results.Ok(transactionType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction type with ID {TransactionTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateTransactionTypeAsync(TransactionTypeDto transactionType)
        {
            var validationResult = await _validator.ValidateAsync(transactionType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newTransactionType = await _transactionTypeService.CreateTransactionTypeAsync(transactionType).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/transaction-types/{newTransactionType.TypeId}", newTransactionType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction type");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateTransactionTypeAsync(Guid id, TransactionTypeDto transactionType)
        {
            if (id != transactionType.TypeId)
                return Results.BadRequest("Transaction Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(transactionType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedTransactionType = await _transactionTypeService.UpdateTransactionTypeAsync(transactionType).ConfigureAwait(false);
                if (updatedTransactionType is not null)
                {
                    //    // Clear cache for the list of system settings
                    //    _ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedTransactionType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction type with ID {TransactionTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteTransactionTypeAsync(Guid id)
        {
            try
            {
                var deletedTransactionType = await _transactionTypeService.DeleteTransactionTypeAsync(id).ConfigureAwait(false);
                if (deletedTransactionType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedTransactionType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction type with ID {TransactionTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
