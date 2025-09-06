using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class FarmEndpoints
    {
        private readonly FarmService _farmService;
        private readonly IValidator<FarmDto> _validator;
        private readonly ILogger<FarmEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "farm";

        public FarmEndpoints(FarmService farmService, IValidator<FarmDto> validator, ILogger<FarmEndpoints> logger)
        {
            _farmService = farmService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllFarmsAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _farmService.GetAllFarmsAsync("FarmContracts.Customer", "Staffs", "FarmZones.Soil", "FarmInventories.Inventory.Unit",
                                                                "FarmLivestocks.Livestock", "FarmLivestocks.Zone", "FarmCrops.Crop", "FarmCrops.Zone").ConfigureAwait(false);
                //    return result;
                //});

                if (!result.Any())
                    return Results.NotFound();

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farms");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmByIdAsync(Guid id)
        {
            try
            {
                var farm = await _farmService.GetFarmByIdAsync(id, "FarmContracts.Customer", "Staffs", "FarmZones.Soil", "FarmInventories.Inventory.Unit",
                                                                   "FarmLivestocks.Livestock", "FarmLivestocks.Zone", "FarmCrops.Crop", "FarmCrops.Zone").ConfigureAwait(false);
                if (farm is null)
                    return Results.NotFound();

                return farm is not null ? Results.Ok(farm) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm with ID {FarmId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmAsync(FarmDto farm)
        {
            var validationResult = await _validator.ValidateAsync(farm).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newFarm = await _farmService.CreateFarmAsync(farm).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/farms/{newFarm.FarmId}", newFarm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmAsync(Guid id, FarmDto farm)
        {
            if (id != farm.FarmId)
                return Results.BadRequest("Farm ID mismatch");

            var validationResult = await _validator.ValidateAsync(farm).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedFarm = await _farmService.UpdateFarmAsync(farm).ConfigureAwait(false);
                if (updatedFarm is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedFarm);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm with ID {FarmId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmAsync(Guid id)
        {
            try
            {
                var deletedFarm = await _farmService.DeleteFarmAsync(id).ConfigureAwait(false);
                if (deletedFarm is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedFarm);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm with ID {FarmId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CompleteFarmSetupAsync(FarmSetupDto farm)
        {
            try
            {
                var farmObj = await _farmService.CompleteFarmSetupAsync(farm).ConfigureAwait(false);
                if (farmObj is not null)
                {
                    return Results.Ok(farmObj);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing setup for farm with ID {FarmId}", farm.FarmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> IsFarmSetupCompleteAsync(Guid id)
        {
            try
            {
                var farmObj = await _farmService.GetFarmByIdAsync(id).ConfigureAwait(false);
                if (farmObj is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(farmObj.SetupComplete);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking setup completion for farm with ID {FarmId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
