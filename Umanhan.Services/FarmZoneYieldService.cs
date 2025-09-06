using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmZoneYieldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmZoneYieldService> _logger;

        private async Task<List<FarmZoneYieldDto>> ToFarmZoneYieldDto(IEnumerable<FarmZoneYield> farmZoneYields)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return [.. farmZoneYields.Select(x => new FarmZoneYieldDto
            {
                YieldId = x.Id,
                ActualYield = x.ActualYield,
                ExpectedYield = x.ExpectedYield,
                ZoneId = x.ZoneId,
                ForecastedYield = x.ForecastedYield,
                ProductId = x.ProductId,
                ContractDetailId = x.ContractDetailId,
                UnitId = x.UnitId,
                UnitName = x.Unit?.UnitName,
                ProductName = productLookup[new ProductKey(x.ProductId, x.ProductTypeId)].ProductName,
                ZoneName = x.Zone?.ZoneName,
                ProductTypeId = x.ProductTypeId,
                ProductTypeName = x.ProductType.ProductTypeName,
                HarvestDate = x.HarvestDate,
                FarmId = x.FarmId
            })];
        }

        private async Task<FarmZoneYieldDto> ToFarmZoneYieldDto(FarmZoneYield farmZoneYield)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return new FarmZoneYieldDto
            {
                YieldId = farmZoneYield.Id,
                ActualYield = farmZoneYield.ActualYield,
                ExpectedYield = farmZoneYield.ExpectedYield,
                ZoneId = farmZoneYield.ZoneId,
                ForecastedYield = farmZoneYield.ForecastedYield,
                ProductId = farmZoneYield.ProductId,
                ContractDetailId = farmZoneYield.ContractDetailId,
                UnitId = farmZoneYield.UnitId,
                UnitName = farmZoneYield.Unit?.UnitName,
                ProductName = productLookup[new ProductKey(farmZoneYield.ProductId, farmZoneYield.ProductTypeId)].ProductName,
                ZoneName = farmZoneYield.Zone?.ZoneName,
                ProductTypeId = farmZoneYield.ProductTypeId,
                ProductTypeName = farmZoneYield.ProductType.ProductTypeName,
                HarvestDate = farmZoneYield.HarvestDate,
                FarmId = farmZoneYield.FarmId
            };
        }

        public FarmZoneYieldService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmZoneYieldService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmZoneYieldDto>> GetAllFarmZoneYieldsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmZoneYields.GetAllAsync(includeEntities).ConfigureAwait(false);
            return await ToFarmZoneYieldDto(list);
        }

        public async Task<FarmZoneYieldDto> GetFarmZoneYieldByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmZoneYields.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return await ToFarmZoneYieldDto(obj);
        }

        public async Task<FarmZoneYieldDto> CreateFarmZoneYieldAsync(FarmZoneYieldDto farmZoneYield)
        {
            var newFarmZoneYield = new FarmZoneYield
            {
                ActualYield = farmZoneYield.ActualYield,
                ExpectedYield = farmZoneYield.ExpectedYield,
                ZoneId = farmZoneYield.ZoneId,
                ForecastedYield = farmZoneYield.ForecastedYield,
                ProductId = farmZoneYield.ProductId,
                UnitId = farmZoneYield.UnitId,
                ContractDetailId = farmZoneYield.ContractDetailId,
                ProductTypeId = farmZoneYield.ProductTypeId,
                HarvestDate = farmZoneYield.HarvestDate,
                FarmId = farmZoneYield.FarmId
            };

            try
            {
                var createdFarmZoneYield = await _unitOfWork.FarmZoneYields.AddAsync(newFarmZoneYield).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmZoneYieldDto(createdFarmZoneYield);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmZoneYield");
                throw;
            }
        }

        public async Task<FarmZoneYieldDto> UpdateFarmZoneYieldAsync(FarmZoneYieldDto farmZoneYield)
        {
            var farmZoneYieldEntity = await _unitOfWork.FarmZoneYields.GetByIdAsync(farmZoneYield.YieldId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmZoneYield not found.");
            farmZoneYieldEntity.ActualYield = farmZoneYield.ActualYield;
            farmZoneYieldEntity.ExpectedYield = farmZoneYield.ExpectedYield;
            farmZoneYieldEntity.ZoneId = farmZoneYield.ZoneId;
            farmZoneYieldEntity.ForecastedYield = farmZoneYield.ForecastedYield;
            farmZoneYieldEntity.ProductId = farmZoneYield.ProductId;
            farmZoneYieldEntity.UnitId = farmZoneYield.UnitId;
            farmZoneYieldEntity.ContractDetailId = farmZoneYield.ContractDetailId;
            farmZoneYieldEntity.ProductTypeId = farmZoneYield.ProductTypeId;
            farmZoneYieldEntity.HarvestDate = farmZoneYield.HarvestDate;
            farmZoneYieldEntity.FarmId = farmZoneYield.FarmId;

            try
            {
                var updatedFarmZoneYield = await _unitOfWork.FarmZoneYields.UpdateAsync(farmZoneYieldEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmZoneYieldDto(updatedFarmZoneYield);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmZoneYield");
                throw;
            }
        }

        public async Task<FarmZoneYieldDto> DeleteFarmZoneYieldAsync(Guid id)
        {
            var farmZoneYieldEntity = await _unitOfWork.FarmZoneYields.GetByIdAsync(id).ConfigureAwait(false);
            if (farmZoneYieldEntity == null)
                return null;

            try
            {
                var deletedFarmZoneYield = await _unitOfWork.FarmZoneYields.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return await ToFarmZoneYieldDto(new FarmZoneYield());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmZoneYield: {Message}", ex.Message);
                throw;
            }
        }
    }
}
