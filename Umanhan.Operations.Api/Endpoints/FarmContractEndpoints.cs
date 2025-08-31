using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmContractEndpoints
    {
        private readonly FarmContractService _farmContractService;
        private readonly IValidator<FarmContractDto> _validator;
        private readonly ILogger<FarmContractEndpoints> _logger;

        public FarmContractEndpoints(FarmContractService farmContractService, IValidator<FarmContractDto> validator, ILogger<FarmContractEndpoints> logger)
        {
            _farmContractService = farmContractService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmContractsAsync(Guid farmId)
        {
            try
            {
                var farmContracts = await _farmContractService.GetFarmContractsAsync(farmId, "Farm", "Customer", "FarmContractDetails.ProductType", "FarmContractDetails.Unit", "FarmActivities").ConfigureAwait(false);
                return Results.Ok(farmContracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all farm contracts");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmContractsAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var farmContracts = await _farmContractService.GetFarmContractsAsync(farmId, startDate, endDate, "Farm", "Customer", "FarmContractDetails.ProductType", "FarmContractDetails.Unit", "FarmActivities").ConfigureAwait(false);
                return Results.Ok(farmContracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all farm contracts");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmContractByIdAsync(Guid id)
        {
            try
            {
                var farmContract = await _farmContractService.GetFarmContractByIdAsync(id, "Farm", "Customer", "FarmContractDetails.ProductType", "FarmContractDetails.Unit", "FarmActivities").ConfigureAwait(false);
                return farmContract is not null ? Results.Ok(farmContract) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm contract by ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmContractAsync(FarmContractDto farmContract)
        {
            var validationResult = await _validator.ValidateAsync(farmContract).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmContract = await _farmContractService.CreateFarmContractAsync(farmContract).ConfigureAwait(false);
                return Results.Created($"/api/farm-contracts/{newFarmContract.ContractId}", newFarmContract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm contract");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmContractAsync(Guid id, FarmContractDto farmContract)
        {
            if (id != farmContract.ContractId)
            {
                return Results.BadRequest("Farm Contract ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmContract).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmContract = await _farmContractService.UpdateFarmContractAsync(farmContract).ConfigureAwait(false);
                return updatedFarmContract is not null ? Results.Ok(updatedFarmContract) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm contract with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmContractAsync(Guid id)
        {
            try
            {
                var deletedFarmContract = await _farmContractService.DeleteFarmContractAsync(id).ConfigureAwait(false);
                return deletedFarmContract is not null ? Results.Ok(deletedFarmContract) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm contract with ID: {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}