using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmInventoryService> _logger;

        private static List<FarmInventoryDto> ToFarmInventoryDto(IEnumerable<FarmInventory> farmInventories)
        {
            return [.. farmInventories.Select(x => new FarmInventoryDto
            {
                FarmInventoryId = x.Id,
                FarmId = x.FarmId,
                UnitId = x.UnitId,
                InventoryId = x.InventoryId,
                Quantity = x.Quantity,
                Status = x.Status,
                Notes = x.Notes,
                FarmLocation = x.Farm?.Location,
                FarmName = x.Farm?.FarmName,
                InventoryCategory = x.Inventory?.Category?.CategoryName,
                InventoryItemName = x.Inventory?.ItemName,
                InventoryUnit = x.Inventory?.Unit?.UnitName,
                InventoryCategoryGroup = x.Inventory?.Category?.Group,
                InventoryCategoryGroup2 = x.Inventory?.Category?.Group2,
                InventoryCategoryConsumptionBehavior = x.Inventory?.Category?.ConsumptionBehavior,
                InventoryItemImageThumbnail = x.ItemImageS3KeyThumbnail,
                InventoryItemImageFull = x.ItemImageS3KeyFull,
                InventoryItemImageContentType = x.ItemImageS3ContentType,
                InventoryItemImageS3UrlFull = x.ItemImageS3UrlFull,
                InventoryItemImageS3UrlThumbnail = x.ItemImageS3UrlThumbnail
            })
            .OrderBy(x => x.InventoryItemName)];
        }

        private static FarmInventoryDto ToFarmInventoryDto(FarmInventory farmInventory)
        {
            return new FarmInventoryDto
            {
                FarmInventoryId = farmInventory.Id,
                FarmId = farmInventory.FarmId,
                UnitId = farmInventory.UnitId,
                InventoryId = farmInventory.InventoryId,
                Quantity = farmInventory.Quantity,
                Status = farmInventory.Status,
                Notes = farmInventory.Notes,
                FarmLocation = farmInventory.Farm?.Location,
                FarmName = farmInventory.Farm?.FarmName,
                InventoryCategory = farmInventory.Inventory?.Category?.CategoryName,
                InventoryItemName = farmInventory.Inventory?.ItemName,
                InventoryUnit = farmInventory.Inventory?.Unit?.UnitName,
                InventoryCategoryGroup = farmInventory.Inventory?.Category?.Group,
                InventoryCategoryGroup2 = farmInventory.Inventory?.Category?.Group2,
                InventoryCategoryConsumptionBehavior = farmInventory.Inventory?.Category?.ConsumptionBehavior,
                InventoryItemImageThumbnail = farmInventory.ItemImageS3KeyThumbnail,
                InventoryItemImageFull = farmInventory.ItemImageS3KeyFull,
                InventoryItemImageContentType = farmInventory.ItemImageS3ContentType,
                InventoryItemImageS3UrlFull = farmInventory.ItemImageS3UrlFull,
                InventoryItemImageS3UrlThumbnail = farmInventory.ItemImageS3UrlThumbnail
            };
        }

        public FarmInventoryService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmInventoryService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmInventoryDto>> GetAllFarmInventoriesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmInventories.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmInventoryDto(list);
        }

        public async Task<IEnumerable<FarmInventoryDto>> GetFarmInventoriesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmInventories.GetFarmInventoriesAsync(farmId, includeEntities).ConfigureAwait(false);
            return ToFarmInventoryDto(list);
        }

        public async Task<FarmInventoryDto> GetFarmInventoryByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmInventories.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmInventoryDto(obj);
        }

        public async Task<FarmInventoryDto> CreateFarmInventoryAsync(FarmInventoryDto farmInventory)
        {
            var newFarmInventory = new FarmInventory
            {
                FarmId = farmInventory.FarmId,
                UnitId = farmInventory.UnitId,
                InventoryId = farmInventory.InventoryId,
                Quantity = farmInventory.Quantity,
                Status = farmInventory.Status,
                Notes = farmInventory.Notes,
            };

            try
            {
                var createdFarmInventory = await _unitOfWork.FarmInventories.AddAsync(newFarmInventory).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmInventoryDto(createdFarmInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Farm Inventory");
                throw;
            }
        }

        public async Task<FarmInventoryDto> UpdateFarmInventoryAsync(FarmInventoryDto farmInventory)
        {
            var farmInventoryEntity = await _unitOfWork.FarmInventories.GetByIdAsync(farmInventory.FarmInventoryId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Inventory not found.");
            farmInventoryEntity.Status = farmInventory.Status;
            farmInventoryEntity.Quantity = farmInventory.Quantity;
            farmInventoryEntity.Notes = farmInventory.Notes;
            farmInventoryEntity.FarmId = farmInventory.FarmId;
            farmInventoryEntity.UnitId = farmInventory.UnitId;
            farmInventoryEntity.InventoryId = farmInventory.InventoryId;

            try
            {
                var updatedFarmInventory = await _unitOfWork.FarmInventories.UpdateAsync(farmInventoryEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmInventoryDto(updatedFarmInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm Inventory");
                throw;
            }
        }

        public async Task<FarmInventoryDto> UpdateFarmInventoryPhotoAsync(FarmInventoryDto farmInventory)
        {
            try
            {
                var farmInventoryEntity = await _unitOfWork.FarmInventories.GetByIdAsync(farmInventory.FarmInventoryId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Inventory not found.");
                farmInventoryEntity.Id = farmInventory.FarmInventoryId;
                farmInventoryEntity.ItemImageS3ContentType = farmInventory.InventoryItemImageContentType;
                farmInventoryEntity.ItemImageS3KeyFull = farmInventory.InventoryItemImageFull;
                farmInventoryEntity.ItemImageS3UrlFull = farmInventory.InventoryItemImageS3UrlFull;
                farmInventoryEntity.ItemImageS3KeyThumbnail = farmInventory.InventoryItemImageThumbnail;
                farmInventoryEntity.ItemImageS3UrlThumbnail = farmInventory.InventoryItemImageS3UrlThumbnail;

                var updatedFarmInventory = await _unitOfWork.FarmInventories.UpdateAsync(farmInventoryEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmInventoryDto(updatedFarmInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm Inventory photo: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmInventoryDto> DeleteFarmInventoryAsync(Guid id)
        {
            var farmInventoryEntity = await _unitOfWork.FarmInventories.GetByIdAsync(id).ConfigureAwait(false);
            if (farmInventoryEntity == null)
                return null;

            try
            {
                var deletedFarmInventory = await _unitOfWork.FarmInventories.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmInventoryDto(new FarmInventory());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Farm Inventory: {Message}", ex.Message);
                throw;
            }
        }
    }
}
