using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmCropService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmCropService> _logger;

        private static List<FarmCropDto> ToFarmCropDto(IEnumerable<FarmCrop> farmCrops)
        {
            return [.. farmCrops.Select(x => new FarmCropDto
            {
                FarmCropId = x.Id,
                FarmId = x.FarmId,
                CropId = x.CropId,
                UnitId = x.UnitId,
                DefaultRate = x.DefaultRate,
                EstimatedHarvestDateUtc = x.EstimatedHarvestDate,
                PlantingDateUtc = x.PlantingDate,
                ZoneId = x.ZoneId,
                ZoneName = x.Zone?.ZoneName,
                ZoneIrrigationType = x.Zone?.IrrigationType,
                ZoneSizeInHectares = x.Zone?.SizeInHectares,
                ZoneSoilType = x.Zone?.Soil?.SoilName,
                CropName = x.Crop?.CropName,
                CropVariety = x.Crop?.CropVariety,
                FarmName = x.Farm?.FarmName,
                FarmLocation = x.Farm?.Location,
                CropUnit = x.Crop?.DefaultUnit?.UnitName,
            })];
        }

        private static FarmCropDto ToFarmCropDto(FarmCrop farmCrop)
        {
            return new FarmCropDto
            {
                FarmCropId = farmCrop.Id,
                FarmId = farmCrop.FarmId,
                CropId = farmCrop.CropId,
                UnitId = farmCrop.UnitId,
                DefaultRate = farmCrop.DefaultRate,
                EstimatedHarvestDateUtc = farmCrop.EstimatedHarvestDate,
                PlantingDateUtc = farmCrop.PlantingDate,
                ZoneId = farmCrop.ZoneId,
                ZoneName = farmCrop.Zone?.ZoneName,
                ZoneIrrigationType = farmCrop.Zone?.IrrigationType,
                ZoneSizeInHectares = farmCrop.Zone?.SizeInHectares,
                ZoneSoilType = farmCrop.Zone?.Soil?.SoilName,
                CropName = farmCrop.Crop?.CropName,
                CropVariety = farmCrop.Crop?.CropVariety,
                FarmName = farmCrop.Farm?.FarmName,
                FarmLocation = farmCrop.Farm?.Location,
                CropUnit = farmCrop.Crop?.DefaultUnit?.UnitName,
            };
        }

        public FarmCropService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmCropService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmCropDto>> GetAllFarmCropsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmCrops.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmCropDto(list);
        }

        public async Task<FarmCropDto> GetFarmCropByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmCrops.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmCropDto(obj);
        }

        public async Task<FarmCropDto> GetFarmCropByCropIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmCrops.GetByCropAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmCropDto(obj);
        }

        public async Task<FarmCropDto> CreateFarmCropAsync(FarmCropDto farmCrop)
        {
            var newFarmCrop = new FarmCrop
            {
                CropId = farmCrop.CropId,
                DefaultRate = farmCrop.DefaultRate,
                EstimatedHarvestDate = farmCrop.EstimatedHarvestDate,
                FarmId = farmCrop.FarmId,
                PlantingDate = farmCrop.PlantingDate,
                UnitId = farmCrop.UnitId,
                ZoneId = farmCrop.ZoneId,
            };

            try
            {
                var createdFarmCrop = await _unitOfWork.FarmCrops.AddAsync(newFarmCrop).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmCropDto(createdFarmCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmCrop");
                throw;
            }
        }

        public async Task<FarmCropDto> UpdateFarmCropAsync(FarmCropDto farmCrop)
        {
            var farmCropEntity = await _unitOfWork.FarmCrops.GetByIdAsync(farmCrop.FarmCropId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmCrop not found.");
            farmCropEntity.DefaultRate = farmCrop.DefaultRate;
            farmCropEntity.PlantingDate = farmCrop.PlantingDate;
            farmCropEntity.EstimatedHarvestDate = farmCrop.EstimatedHarvestDate;
            farmCropEntity.ZoneId = farmCrop.ZoneId;
            farmCropEntity.UnitId = farmCrop.UnitId;
            farmCropEntity.CropId = farmCrop.CropId;

            try
            {
                var updatedFarmCrop = await _unitOfWork.FarmCrops.UpdateAsync(farmCropEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmCropDto(updatedFarmCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmCrop");
                throw;
            }
        }

        public async Task<FarmCropDto> DeleteFarmCropAsync(Guid id)
        {
            var farmCropEntity = await _unitOfWork.FarmCrops.GetByIdAsync(id).ConfigureAwait(false);
            if (farmCropEntity == null)
                return null;

            try
            {
                var deletedFarmCrop = await _unitOfWork.FarmCrops.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmCropDto(new FarmCrop());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmCrop: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmCropDto> CreateUpdateFarmCropAsync(FarmCropDto farmCrop)
        {
            try
            {
                var createdFarmCrop = new FarmCrop();
                var crop = await _unitOfWork.FarmCrops.GetByIdAsync(farmCrop.FarmCropId).ConfigureAwait(false);
                if (crop == null)
                {
                    var newFarmCrop = new FarmCrop
                    {
                        Id = farmCrop.FarmCropId,
                        CropId = farmCrop.CropId,
                        DefaultRate = farmCrop.DefaultRate,
                        EstimatedHarvestDate = farmCrop.EstimatedHarvestDate,
                        FarmId = farmCrop.FarmId,
                        PlantingDate = farmCrop.PlantingDate,
                        UnitId = farmCrop.UnitId,
                        ZoneId = farmCrop.ZoneId,
                    };
                    createdFarmCrop = await _unitOfWork.FarmCrops.AddAsync(newFarmCrop).ConfigureAwait(false);
                }
                else
                {
                    crop.PlantingDate = farmCrop.PlantingDate;
                    crop.EstimatedHarvestDate = farmCrop.EstimatedHarvestDate;
                    crop.ZoneId = farmCrop.ZoneId;
                    crop.UnitId = farmCrop.UnitId;
                    crop.CropId = farmCrop.CropId;
                    crop.DefaultRate = farmCrop.DefaultRate;
                    crop.FarmId = farmCrop.FarmId;

                    createdFarmCrop = await _unitOfWork.FarmCrops.UpdateAsync(crop).ConfigureAwait(false);
                }

                // commit changes only when all operations are successful
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmCropDto(createdFarmCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating or updating FarmCrop: {Message}", ex.Message);
                throw;
            }
        }
    }
}
