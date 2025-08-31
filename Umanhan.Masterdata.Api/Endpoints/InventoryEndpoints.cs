using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class InventoryEndpoints
    {
        private readonly InventoryService _inventoryService;
        private readonly IValidator<InventoryDto> _validator;
        private readonly ILogger<InventoryEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "inventory";

        public InventoryEndpoints(InventoryService inventoryService, IValidator<InventoryDto> validator, ILogger<InventoryEndpoints> logger)
        {
            _inventoryService = inventoryService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllInventoriesAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _inventoryService.GetAllInventoriesAsync("Category", "Unit").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventories");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetInventoryByIdAsync(Guid id)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByIdAsync(id, "Category", "Unit").ConfigureAwait(false);
                return inventory is not null ? Results.Ok(inventory) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory with ID {InventoryId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateInventoryAsync(InventoryDto inventory)
        {
            var validationResult = await _validator.ValidateAsync(inventory).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newInventory = await _inventoryService.CreateInventoryAsync(inventory).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/inventories/{newInventory.InventoryId}", newInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateInventoryAsync(Guid id, InventoryDto inventory)
        {
            if (id != inventory.InventoryId)
                return Results.BadRequest("Inventory ID mismatch");

            var validationResult = await _validator.ValidateAsync(inventory).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedInventory = await _inventoryService.UpdateInventoryAsync(inventory).ConfigureAwait(false);
                if (updatedInventory is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedInventory);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory with ID {InventoryId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteInventoryAsync(Guid id)
        {
            try
            {
                var deletedInventory = await _inventoryService.DeleteInventoryAsync(id).ConfigureAwait(false);
                if (deletedInventory is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedInventory);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory with ID {InventoryId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
