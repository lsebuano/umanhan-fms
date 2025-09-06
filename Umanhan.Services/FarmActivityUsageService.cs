using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmActivityUsageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmActivityUsageService> _logger;

        private static List<FarmActivityUsageDto> ToFarmActivityUsageDto(IEnumerable<FarmActivityUsage> farmActivityUsages)
        {
            return [.. farmActivityUsages.Select(x => new FarmActivityUsageDto
            {
                UsageId = x.Id,
                ActivityId = x.ActivityId,
                InventoryId = x.InventoryId,
                ItemName = x.Inventory?.ItemName,
                Rate = x.Rate,
                TotalCost = x.TotalCost,
                UsageHours = x.UsageHours,
                UnitId = x.Inventory?.UnitId,
                Unit = x.Inventory?.Unit?.UnitName,
                //FarmId = x.Activity.FarmId,
                Timestamp = x.Timestamp
            })];
        }

        private static FarmActivityUsageDto ToFarmActivityUsageDto(FarmActivityUsage farmActivityUsage)
        {
            return new FarmActivityUsageDto
            {
                UsageId = farmActivityUsage.Id,
                ActivityId = farmActivityUsage.ActivityId,
                InventoryId = farmActivityUsage.InventoryId,
                ItemName = farmActivityUsage.Inventory?.ItemName,
                Rate = farmActivityUsage.Rate,
                TotalCost = farmActivityUsage.TotalCost,
                UsageHours = farmActivityUsage.UsageHours,
                UnitId = farmActivityUsage.Inventory?.UnitId,
                Unit = farmActivityUsage.Inventory?.Unit?.UnitName,
                //FarmId = farmActivityUsage?.Activity?.FarmId,
                Timestamp = farmActivityUsage.Timestamp
            };
        }

        public FarmActivityUsageService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmActivityUsageService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmActivityUsageDto>> GetFarmActivityUsagesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityUsages.GetFarmActivityUsagesAsync(farmId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityUsageDto(list);
        }

        public async Task<IEnumerable<FarmActivityUsageDto>> GetFarmActivityUsagesByItemAsync(Guid farmId, Guid itemId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityUsages.GetFarmActivityUsagesByItemAsync(farmId, itemId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityUsageDto(list);
        }

        public async Task<IEnumerable<FarmActivityUsageDto>> GetFarmActivityUsagesByActivityAsync(Guid activityId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityUsages.GetFarmActivityUsagesByActivityAsync(activityId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityUsageDto(list);
        }

        public async Task<FarmActivityUsageDto> GetFarmActivityUsageByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmActivityUsages.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmActivityUsageDto(obj);
        }

        public async Task<FarmActivityUsageDto> CreateFarmActivityUsageAsync(FarmActivityUsageDto farmActivityUsage)
        {
            var newFarmActivityUsage = new FarmActivityUsage
            {
                ActivityId = farmActivityUsage.ActivityId,
                InventoryId = farmActivityUsage.InventoryId,
                Rate = farmActivityUsage.Rate,
                TotalCost = farmActivityUsage.TotalCost,
                UsageHours = farmActivityUsage.UsageHours,
                UnitId = farmActivityUsage.UnitId
            };

            try
            {
                // deduct usage to inventory
                var inventory = await _unitOfWork.FarmInventories.GetFarmInventoryByIdAsync(farmActivityUsage.FarmId, farmActivityUsage.InventoryId).ConfigureAwait(false);
                if (inventory == null)
                    throw new Exception("Inventory item not found.");

                // check if the inventory has enough quantity
                if (inventory.Quantity < farmActivityUsage.UsageHours)
                    throw new Exception("Not enough inventory quantity for this usage.");

                // Update inventory quantity
                inventory.Quantity -= farmActivityUsage.UsageHours;

                // Create the farm activity usage
                var createdFarmActivityUsage = await _unitOfWork.FarmActivityUsages.AddAsync(newFarmActivityUsage).ConfigureAwait(false);
                if (createdFarmActivityUsage == null)
                    throw new Exception("Failed to create Farm Activity Usage.");

                // Update the inventory
                await _unitOfWork.FarmInventories.UpdateAsync(inventory).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityUsageDto(createdFarmActivityUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmActivityUsage: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityUsageDto> UpdateFarmActivityUsageAsync(FarmActivityUsageDto farmActivityUsage)
        {
            var farmActivityUsageEntity = await _unitOfWork.FarmActivityUsages.GetByIdAsync(farmActivityUsage.UsageId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmActivityUsage not found.");
            farmActivityUsageEntity.TotalCost = farmActivityUsage.TotalCost;
            farmActivityUsageEntity.Rate = farmActivityUsage.Rate;
            farmActivityUsageEntity.UsageHours = farmActivityUsage.UsageHours;
            farmActivityUsageEntity.ActivityId = farmActivityUsage.ActivityId;
            farmActivityUsageEntity.InventoryId = farmActivityUsage.InventoryId;

            try
            {
                var updatedFarmActivityUsage = await _unitOfWork.FarmActivityUsages.UpdateAsync(farmActivityUsageEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityUsageDto(updatedFarmActivityUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmActivityUsage: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityUsageDto> DeleteFarmActivityUsageAsync(Guid id)
        {
            var farmActivityUsageEntity = await _unitOfWork.FarmActivityUsages.GetByIdAsync(id).ConfigureAwait(false);
            if (farmActivityUsageEntity == null)
                return null;

            try
            {
                var deletedFarmActivityUsage = await _unitOfWork.FarmActivityUsages.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityUsageDto(new FarmActivityUsage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmActivityUsage: {Message}", ex.Message);
                throw;
            }
        }
    }
}
