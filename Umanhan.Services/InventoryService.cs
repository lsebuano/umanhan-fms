using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class InventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<InventoryService> _logger;

        private static List<InventoryDto> ToInventoryDto(IEnumerable<Inventory> inventories)
        {
            return [.. inventories.Select(x => new InventoryDto
            {
                InventoryId = x.Id,
                UnitId = x.UnitId,
                ItemName = x.ItemName,
                CategoryId = x.CategoryId,
                Category = x.Category?.CategoryName,
                Unit = x.Unit?.UnitName,
            })
            .OrderBy(x => x.ItemName)];
        }

        private static InventoryDto ToInventoryDto(Inventory inventory)
        {
            return new InventoryDto
            {
                InventoryId = inventory.Id,
                UnitId = inventory.UnitId,
                ItemName = inventory.ItemName,
                CategoryId = inventory.CategoryId,
                Category = inventory.Category?.CategoryName,
                Unit = inventory.Unit?.UnitName,
            };
        }

        public InventoryService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<InventoryService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Inventories.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToInventoryDto(list);
        }

        public async Task<InventoryDto> GetInventoryByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Inventories.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToInventoryDto(obj);
        }

        public async Task<InventoryDto> CreateInventoryAsync(InventoryDto inventory)
        {
            var newInventory = new Inventory
            {
                UnitId = inventory.UnitId,
                ItemName = inventory.ItemName,
                CategoryId = inventory.CategoryId,
            };

            try
            {
                var createdInventory = await _unitOfWork.Inventories.AddAsync(newInventory).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToInventoryDto(createdInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                throw;
            }
        }

        public async Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventory)
        {
            var inventoryEntity = await _unitOfWork.Inventories.GetByIdAsync(inventory.InventoryId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Inventory not found.");
            inventoryEntity.UnitId = inventory.UnitId;
            inventoryEntity.ItemName = inventory.ItemName;
            inventoryEntity.CategoryId = inventory.CategoryId;

            try
            {
                var updatedInventory = await _unitOfWork.Inventories.UpdateAsync(inventoryEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToInventoryDto(updatedInventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<InventoryDto> DeleteInventoryAsync(Guid id)
        {
            var inventoryEntity = await _unitOfWork.Inventories.GetByIdAsync(id).ConfigureAwait(false);
            if (inventoryEntity == null)
                return null;

            try
            {
                var deletedInventory = await _unitOfWork.Inventories.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToInventoryDto(new Inventory());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory: {Message}", ex.Message);
                throw;
            }
        }
    }
}
