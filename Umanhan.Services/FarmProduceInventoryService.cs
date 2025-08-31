using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmProduceInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmProduceInventoryService> _logger;

        private async Task<List<FarmProduceInventoryDto>> ToFarmProduceInventoryDto(IEnumerable<FarmProduceInventory> farmProduceInventories)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return [.. farmProduceInventories.Select(x => new FarmProduceInventoryDto
            {
                InventoryId = x.Id,
                ProductTypeId = x.ProductTypeId,
                InitialQuantity = x.InitialQuantity,
                UnitId = x.UnitId,
                Date = x.Date.ToDateTime(TimeOnly.MinValue),
                UnitPrice = x.UnitPrice,
                Notes = x.Notes,
                CurrentQuantity = x.CurrentQuantity,
                FarmId = x.FarmId,
                ProductId = x.ProductId,
                ProductType = x.ProductType.ProductTypeName,
                Product = productLookup[new ProductKey(x.ProductId, x.ProductTypeId)].ProductName,
                ProductVariety = productLookup[new ProductKey(x.ProductId, x.ProductTypeId)].Variety,
                Unit = x.Unit.UnitName,
            })];
        }

        private async Task<FarmProduceInventoryDto> ToFarmProduceInventoryDto(FarmProduceInventory farmProduceInventory)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return new FarmProduceInventoryDto
            {
                FarmId = farmProduceInventory.FarmId,
                InventoryId = farmProduceInventory.Id,
                ProductTypeId = farmProduceInventory.ProductTypeId,
                InitialQuantity = farmProduceInventory.InitialQuantity,
                UnitId = farmProduceInventory.UnitId,
                Date = farmProduceInventory.Date.ToDateTime(TimeOnly.MinValue),
                UnitPrice = farmProduceInventory.UnitPrice,
                Notes = farmProduceInventory.Notes,
                CurrentQuantity = farmProduceInventory.CurrentQuantity,
                ProductId = farmProduceInventory.ProductId,
                ProductType = farmProduceInventory.ProductType.ProductTypeName,
                Product = productLookup[new ProductKey(farmProduceInventory.ProductId, farmProduceInventory.ProductTypeId)].ProductName,
                ProductVariety = productLookup[new ProductKey(farmProduceInventory.ProductId, farmProduceInventory.ProductTypeId)].Variety,
                Unit = farmProduceInventory.Unit.UnitName,                
            };
        }

        public FarmProduceInventoryService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmProduceInventoryService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmProduceInventoryDto>> GetFarmProduceInventoriesAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmProduceInventories.GetFarmProduceInventoriesAsync(farmId, includeEntities).ConfigureAwait(false);
            return await ToFarmProduceInventoryDto(list).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmProduceInventoryDto>> GetFarmProduceInventoriesAsync(Guid farmId, Guid typeId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmProduceInventories.GetFarmProduceInventoriesAsync(farmId, typeId, includeEntities).ConfigureAwait(false);
            return await ToFarmProduceInventoryDto(list).ConfigureAwait(false);
        }

        public async Task<FarmProduceInventoryDto> GetFarmProduceInventoryByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmProduceInventories.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return await ToFarmProduceInventoryDto(obj).ConfigureAwait(false);
        }

        public async Task<FarmProduceInventoryDto> CreateFarmProduceInventoryAsync(FarmProduceInventoryDto farmProduceInventory)
        {
            var newFarmProduceInventory = new FarmProduceInventory
            {
                FarmId = farmProduceInventory.FarmId,
                ProductTypeId = farmProduceInventory.ProductTypeId,
                InitialQuantity = farmProduceInventory.InitialQuantity,
                UnitId = farmProduceInventory.UnitId,
                Date = DateOnly.FromDateTime(farmProduceInventory.Date),
                UnitPrice = farmProduceInventory.UnitPrice,
                Notes = farmProduceInventory.Notes,
                CurrentQuantity = farmProduceInventory.CurrentQuantity,
                ProductId = farmProduceInventory.ProductId,
            };

            try
            {
                var createdFarmProduceInventory = await _unitOfWork.FarmProduceInventories.AddAsync(newFarmProduceInventory).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmProduceInventoryDto(createdFarmProduceInventory).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmProduceInventory");
                throw;
            }
        }

        public async Task<FarmProduceInventoryDto> UpdateFarmProduceInventoryAsync(FarmProduceInventoryDto farmProduceInventory)
        {
            var farmProduceInventoryEntity = await _unitOfWork.FarmProduceInventories.GetByIdAsync(farmProduceInventory.InventoryId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmProduceInventory not found.");
            farmProduceInventoryEntity.ProductTypeId = farmProduceInventory.ProductTypeId;
            farmProduceInventoryEntity.InitialQuantity = farmProduceInventory.InitialQuantity;
            farmProduceInventoryEntity.UnitId = farmProduceInventory.UnitId;
            farmProduceInventoryEntity.Date = DateOnly.FromDateTime(farmProduceInventory.Date);
            farmProduceInventoryEntity.UnitPrice = farmProduceInventory.UnitPrice;
            farmProduceInventoryEntity.Notes = farmProduceInventory.Notes;
            farmProduceInventoryEntity.CurrentQuantity = farmProduceInventory.CurrentQuantity;
            farmProduceInventoryEntity.ProductId = farmProduceInventory.ProductId;
            farmProduceInventoryEntity.FarmId = farmProduceInventory.FarmId;

            try
            {
                var updatedFarmProduceInventory = await _unitOfWork.FarmProduceInventories.UpdateAsync(farmProduceInventoryEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmProduceInventoryDto(updatedFarmProduceInventory).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmProduceInventory");
                throw;
            }
        }

        public async Task<FarmProduceInventoryDto> DeleteFarmProduceInventoryAsync(Guid id)
        {
            var farmProduceInventoryEntity = await _unitOfWork.FarmProduceInventories.GetByIdAsync(id).ConfigureAwait(false);
            if (farmProduceInventoryEntity == null)
                return null;

            try
            {
                var deletedFarmProduceInventory = await _unitOfWork.FarmProduceInventories.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmProduceInventoryDto(new FarmProduceInventory()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmProduceInventory: {Message}", ex.Message);
                throw;
            }
        }
    }
}
