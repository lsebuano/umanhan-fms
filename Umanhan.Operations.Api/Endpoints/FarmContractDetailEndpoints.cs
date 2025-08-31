using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmContractDetailEndpoints
    {
        private readonly FarmContractDetailService _farmContractDetailService;
        private readonly IValidator<FarmContractDetailDto> _validator;
        private readonly ILogger<FarmContractDetailEndpoints> _logger;

        public FarmContractDetailEndpoints(FarmContractDetailService farmContractDetailService,
            IValidator<FarmContractDetailDto> validator,
            ILogger<FarmContractDetailEndpoints> logger)
        {
            _farmContractDetailService = farmContractDetailService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmContractDetailsAsync(Guid contractId)
        {
            try
            {
                var farmContractDetails = await _farmContractDetailService.GetFarmContractDetailsAsync(contractId, "Contract", "ProductType", "Unit").ConfigureAwait(false);
                return Results.Ok(farmContractDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all farm contract details");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmContractDetailByIdAsync(Guid id)
        {
            try
            {
                var farmContractDetail = await _farmContractDetailService.GetFarmContractDetailByIdAsync(id, "Contract", "ProductType", "Unit").ConfigureAwait(false);
                return farmContractDetail is not null ? Results.Ok(farmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm contract detail by ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmContractDetailAsync(FarmContractDetailDto farmContractDetail)
        {
            var validationResult = await _validator.ValidateAsync(farmContractDetail).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmContractDetail = await _farmContractDetailService.CreateFarmContractDetailAsync(farmContractDetail).ConfigureAwait(false);
                return Results.Created($"/api/farm-contracts/details/{newFarmContractDetail.ContractDetailId}", newFarmContractDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm contract detail");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmContractDetailAsync(Guid id, FarmContractDetailDto farmContractDetail)
        {
            if (id != farmContractDetail.ContractDetailId)
            {
                return Results.BadRequest("Farm Contract Detail ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmContractDetail).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmContractDetail = await _farmContractDetailService.UpdateFarmContractDetailAsync(farmContractDetail).ConfigureAwait(false);
                return updatedFarmContractDetail is not null ? Results.Ok(updatedFarmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm contract with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmContractDetailAsync(Guid id)
        {
            try
            {
                var deletedFarmContractDetail = await _farmContractDetailService.DeleteFarmContractDetailAsync(id).ConfigureAwait(false);
                return deletedFarmContractDetail is not null ? Results.Ok(deletedFarmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> ConfirmPickupAsync(Guid id)
        {
            try
            {
                var confirmedFarmContractDetail = await _farmContractDetailService.ConfirmPickupAsync(id).ConfigureAwait(false);
                return confirmedFarmContractDetail is not null ? Results.Ok(confirmedFarmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming pickup for farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CancelTransactionAsync(Guid id)
        {
            try
            {
                var cancelledFarmContractDetail = await _farmContractDetailService.CancelTransactionAsync(id).ConfigureAwait(false);
                return cancelledFarmContractDetail is not null ? Results.Ok(cancelledFarmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling transaction for farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> MarkTransactionAsPaidAsync(Guid id)
        {
            try
            {
                var paidFarmContractDetail = await _farmContractDetailService.MarkTransactionAsPaidAsync(id).ConfigureAwait(false);
                return paidFarmContractDetail is not null ? Results.Ok(paidFarmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking transaction as paid for farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> SetHarvestDateAsync(Guid id, DateTime date)
        {
            try
            {
                var farmContractDetail = await _farmContractDetailService.SetHarvestDateAsync(id, date).ConfigureAwait(false);
                return farmContractDetail is not null ? Results.Ok(farmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting harvest date for farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> SetPickupDateAsync(Guid id, DateTime date)
        {
            try
            {
                var farmContractDetail = await _farmContractDetailService.SetPickupDateAsync(id, date).ConfigureAwait(false);
                return farmContractDetail is not null ? Results.Ok(farmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting pickup date for farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> RecoverHarvestAsync(Guid id)
        {
            try
            {
                var farmContractDetail = await _farmContractDetailService.RecoverHarvestAsync(id).ConfigureAwait(false);
                return farmContractDetail is not null ? Results.Ok(farmContractDetail) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recovering harvest for farm contract detail with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}