using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmSalesEndpoints
    {
        private readonly FarmTransactionService _farmTransactionService;
        private readonly IValidator<FarmTransactionDto> _validator;
        private readonly ILogger<FarmSalesEndpoints> _logger;

        public FarmSalesEndpoints(FarmTransactionService farmTransactionService,
            IValidator<FarmTransactionDto> validator,
            ILogger<FarmSalesEndpoints> logger)
        {
            _farmTransactionService = farmTransactionService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmSalesAsync(Guid farmId)
        {
            try
            {
                var farmSaless = await _farmTransactionService.GetFarmSalesAsync(farmId).ConfigureAwait(false);
                return Results.Ok(farmSaless);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm sales for farm ID: {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetRecentFarmTransactionsAsync(Guid farmId)
        {
            try
            {
                var farmSaless = await _farmTransactionService.GetRecentFarmTransactionsAsync(farmId).ConfigureAwait(false);
                return Results.Ok(farmSaless);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent farm sales for farm ID: {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmSalesByIdAsync(Guid id)
        {
            try
            {
                var farmSales = await _farmTransactionService.GetFarmTransactionByIdAsync(id).ConfigureAwait(false);
                return farmSales is not null ? Results.Ok(farmSales) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm sale details by ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmSalesAsync(FarmTransactionDto farmSales)
        {
            var validationResult = await _validator.ValidateAsync(farmSales).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmSales = await _farmTransactionService.CreateFarmTransactionAsync(farmSales).ConfigureAwait(false);
                return Results.Created($"/api/farm-sales/{newFarmSales.TransactionId}", newFarmSales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm sale detail");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmSalesAsync(Guid id, FarmTransactionDto farmSales)
        {
            if (id != farmSales.TransactionId)
            {
                return Results.BadRequest("Farm Sale ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmSales).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmSales = await _farmTransactionService.UpdateFarmTransactionAsync(farmSales).ConfigureAwait(false);
                return updatedFarmSales is not null ? Results.Ok(updatedFarmSales) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm sale with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmSalesAsync(Guid id)
        {
            try
            {
                var deletedFarmSales = await _farmTransactionService.DeleteFarmTransactionAsync(id).ConfigureAwait(false);
                return deletedFarmSales is not null ? Results.Ok(deletedFarmSales) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm sale with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CancelTransactionAsync(Guid id)
        {
            try
            {
                var cancelledFarmSales = await _farmTransactionService.CancelTransactionAsync(id).ConfigureAwait(false);
                return cancelledFarmSales is not null ? Results.Ok(cancelledFarmSales) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling transaction for farm sale with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}