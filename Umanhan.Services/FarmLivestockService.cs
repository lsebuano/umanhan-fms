using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmLivestockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmLivestockService> _logger;

        private static List<FarmLivestockDto> ToFarmLivestockDto(IEnumerable<FarmLivestock> farmLivestocks)
        {
            return [.. farmLivestocks.Select(x => new FarmLivestockDto
            {
                FarmLivestockId = x.Id,
                BirthDateUtc = x.BirthDate,
                Breed = x.Breed,
                DefaultRate = x.DefaultRate,
                FarmId = x.FarmId,
                LivestockId = x.LivestockId,
                PurchaseCost = x.PurchaseCost,
                PurchaseDateUtc = x.PurchaseDate,
                Quantity = x.Quantity,
                UnitId = x.UnitId,
                ZoneId = x.ZoneId,
            })];
        }

        private static FarmLivestockDto ToFarmLivestockDto(FarmLivestock farmLivestock)
        {
            return new FarmLivestockDto
            {
                FarmLivestockId = farmLivestock.Id,
                BirthDateUtc = farmLivestock.BirthDate,
                Breed = farmLivestock.Breed,
                DefaultRate = farmLivestock.DefaultRate,
                FarmId = farmLivestock.FarmId,
                LivestockId = farmLivestock.LivestockId,
                PurchaseCost = farmLivestock.PurchaseCost,
                PurchaseDateUtc = farmLivestock.PurchaseDate,
                Quantity = farmLivestock.Quantity,
                UnitId = farmLivestock.UnitId,
                ZoneId = farmLivestock.ZoneId,
            };
        }

        public FarmLivestockService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmLivestockService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmLivestockDto>> GetAllFarmLivestocksAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmLivestocks.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmLivestockDto(list);
        }

        public async Task<FarmLivestockDto> GetFarmLivestockByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmLivestocks.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmLivestockDto(obj);
        }

        public async Task<FarmLivestockDto> CreateFarmLivestockAsync(FarmLivestockDto farmLivestock)
        {
            var newFarmLivestock = new FarmLivestock
            {
                BirthDate = farmLivestock.BirthDate,
                Breed = farmLivestock.Breed,
                DefaultRate = farmLivestock.DefaultRate,
                FarmId = farmLivestock.FarmId,
                LivestockId = farmLivestock.LivestockId,
                PurchaseCost = farmLivestock.PurchaseCost,
                PurchaseDate = farmLivestock.PurchaseDate,
                Quantity = farmLivestock.Quantity,
                UnitId = farmLivestock.UnitId,
                ZoneId = farmLivestock.ZoneId,
            };

            try
            {
                var createdFarmLivestock = await _unitOfWork.FarmLivestocks.AddAsync(newFarmLivestock).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmLivestockDto(createdFarmLivestock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Farm Livestock");
                throw;
            }
        }

        public async Task<FarmLivestockDto> UpdateFarmLivestockAsync(FarmLivestockDto farmLivestock)
        {
            var farmLivestockEntity = await _unitOfWork.FarmLivestocks.GetByIdAsync(farmLivestock.FarmLivestockId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmLivestock not found.");
            farmLivestockEntity.BirthDate = farmLivestock.BirthDate;
            farmLivestockEntity.Breed = farmLivestock.Breed;
            farmLivestockEntity.DefaultRate = farmLivestock.DefaultRate;
            farmLivestockEntity.FarmId = farmLivestock.FarmId;
            farmLivestockEntity.LivestockId = farmLivestock.LivestockId;
            farmLivestockEntity.PurchaseCost = farmLivestock.PurchaseCost;
            farmLivestockEntity.PurchaseDate = farmLivestock.PurchaseDate;
            farmLivestockEntity.Quantity = farmLivestock.Quantity;
            farmLivestockEntity.UnitId = farmLivestock.UnitId;
            farmLivestockEntity.ZoneId = farmLivestock.ZoneId;

            try
            {
                var updatedFarmLivestock = await _unitOfWork.FarmLivestocks.UpdateAsync(farmLivestockEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmLivestockDto(updatedFarmLivestock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm Livestock: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmLivestockDto> DeleteFarmLivestockAsync(Guid id)
        {
            var farmLivestockEntity = await _unitOfWork.FarmLivestocks.GetByIdAsync(id).ConfigureAwait(false);
            if (farmLivestockEntity == null)
                return null;

            try
            {
                var deletedFarmLivestock = await _unitOfWork.FarmLivestocks.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmLivestockDto(new FarmLivestock());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Farm Livestock: {Message}", ex.Message);
                throw;
            }
        }
    }
}
