using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmProduceInventoryEndpoints
    {
        private readonly FarmProduceInventoryService _farmProduceInventorylService;
        private readonly IValidator<FarmProduceInventoryDto> _validator;
        private readonly ILogger<FarmProduceInventoryEndpoints> _logger;

        public FarmProduceInventoryEndpoints(FarmProduceInventoryService farmProduceInventorylService,
            IValidator<FarmProduceInventoryDto> validator,
            ILogger<FarmProduceInventoryEndpoints> logger)
        {
            _farmProduceInventorylService = farmProduceInventorylService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetFarmProduceInventoriesAsync(Guid farmId)
        {
            try
            {
                var farmProduceInventories = await _farmProduceInventorylService.GetFarmProduceInventoriesAsync(farmId, "ProductType", "Unit").ConfigureAwait(false);
                return Results.Ok(farmProduceInventories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm produce inventories for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmProduceInventoriesAsync(Guid farmId, Guid typeId)
        {
            try
            {
                var farmProduceInventories = await _farmProduceInventorylService.GetFarmProduceInventoriesAsync(farmId, typeId, "ProductType", "Unit").ConfigureAwait(false);
                return Results.Ok(farmProduceInventories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm produce inventories for farm {FarmId} and type {TypeId}", farmId, typeId);
                return Results.Problem(ex.Message);
            }
        }
    }
}
