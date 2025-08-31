using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmContractSaleEndpoints
    {
        private readonly FarmContractSaleService _farmContractSaleService;
        private readonly IValidator<FarmContractSaleDto> _validator;
        private readonly ILogger<FarmContractSaleEndpoints> _logger;

        public FarmContractSaleEndpoints(FarmContractSaleService farmContractSaleService, IValidator<FarmContractSaleDto> validator, ILogger<FarmContractSaleEndpoints> logger)
        {
            _farmContractSaleService = farmContractSaleService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmContractSalesAsync(Guid farmId)
        {
            try
            {
                var farmContractSales = await _farmContractSaleService.GetFarmContractSalesAsync(farmId, "Farm", "Customer", "FarmContractDetails.ProductType", "FarmContractDetails.Unit", "FarmActivities").ConfigureAwait(false);
                return Results.Ok(farmContractSales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all farm contract sales");
                return Results.Problem(ex.Message);
            }
        }
    }
}
