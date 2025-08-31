using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmZoneService> _logger;

        private static List<FarmZoneDto> ToFarmZoneDto(IEnumerable<FarmZone> farmZones)
        {
            return [.. farmZones.Select(x => new FarmZoneDto
            {
                ZoneId = x.Id,
                FarmZoneNotes = x.Notes,
                ZoneName = x.ZoneName,
                FarmId = x.FarmId,
                FarmName = x.Farm?.FarmName,
                IrrigationType = x.IrrigationType,
                SizeInHectares = x.SizeInHectares,
                SoilId = x.SoilId,
                ZoneBoundaryJson = x.BoundaryJson,
                ZoneColor = x.ZoneColor,
                ZoneCentroidLat = x.Lat,
                ZoneCentroidLng = x.Lng,
                FarmSizeInHectares = x.Farm?.SizeInHectares,
                FarmSizeInSqm = x.Farm?.SizeInSqm,
                FarmLocation = x.Farm?.Location,
                FarmFullAddress = x.Farm?.FullAddress,
                FarmCentroidLat = Convert.ToDouble(x.Farm?.Lat ?? 0),
                FarmCentroidLng = Convert.ToDouble(x.Farm?.Lng ?? 0),
                FarmBoundaryJson = x.Farm?.BoundaryJson,
                FarmStaticMapUrl = x.Farm?.StaticMapUrl,
                FarmSetupComplete = x.Farm?.SetupComplete ?? false,
                AreaInSqm = x.SizeInSqm,
                Description = x.ZoneDescription,
                SoilType = x.Soil?.SoilName,
            })
            .OrderBy(x => x.ZoneName)];
        }

        private static FarmZoneDto ToFarmZoneDto(FarmZone farmZone)
        {
            return new FarmZoneDto
            {
                ZoneId = farmZone.Id,
                FarmZoneNotes = farmZone.Notes,
                ZoneName = farmZone.ZoneName,
                FarmId = farmZone.FarmId,
                FarmName = farmZone.Farm?.FarmName,
                IrrigationType = farmZone.IrrigationType,
                SizeInHectares = farmZone.SizeInHectares,
                SoilId = farmZone.SoilId,
                ZoneBoundaryJson = farmZone.BoundaryJson,
                ZoneColor = farmZone.ZoneColor,
                ZoneCentroidLat = farmZone.Lat,
                ZoneCentroidLng = farmZone.Lng,
                FarmSizeInHectares = farmZone.Farm?.SizeInHectares,
                FarmSizeInSqm = farmZone.Farm?.SizeInSqm,
                FarmLocation = farmZone.Farm?.Location,
                FarmFullAddress = farmZone.Farm?.FullAddress,
                FarmCentroidLat = Convert.ToDouble(farmZone.Farm?.Lat ?? 0),
                FarmCentroidLng = Convert.ToDouble(farmZone.Farm?.Lng ?? 0),
                FarmBoundaryJson = farmZone.Farm?.BoundaryJson,
                FarmStaticMapUrl = farmZone.Farm?.StaticMapUrl,
                FarmSetupComplete = farmZone.Farm?.SetupComplete ?? false,
                AreaInSqm = farmZone.SizeInSqm,
                Description = farmZone.ZoneDescription,
                SoilType = farmZone.Soil?.SoilName,
            };
        }

        public FarmZoneService(IUnitOfWork unitOfWork,
            IUserContextService userContext,
            ILogger<FarmZoneService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmZoneDto>> GetAllFarmZonesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmZones.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmZoneDto(list);
        }

        public async Task<IEnumerable<FarmZoneDto>> GetFarmZonesByFarmAsync(Guid farmId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmZones.GetFarmZonesByFarmAsync(farmId, includeEntities).ConfigureAwait(false);
            return ToFarmZoneDto(list);
        }

        public async Task<FarmZoneDto> GetFarmZoneByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmZones.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmZoneDto(obj);
        }

        public async Task<FarmZoneDto> CreateFarmZoneAsync(FarmZoneDto farmZone)
        {
            var newFarmZone = new FarmZone
            {
                Notes = farmZone.FarmZoneNotes,
                ZoneName = farmZone.ZoneName,
                FarmId = farmZone.FarmId,
                IrrigationType = farmZone.IrrigationType,
                SizeInHectares = farmZone.SizeInHectares,
                SoilId = farmZone.SoilId,
            };

            try
            {
                var createdFarmZone = await _unitOfWork.FarmZones.AddAsync(newFarmZone).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmZoneDto(createdFarmZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmZone");
                throw;
            }
        }

        public async Task<FarmZoneDto> UpdateFarmZoneAsync(FarmZoneDto farmZone)
        {
            var farmZoneEntity = await _unitOfWork.FarmZones.GetByIdAsync(farmZone.ZoneId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmZone not found.");

            farmZoneEntity.Notes = farmZone.FarmZoneNotes;
            farmZoneEntity.ZoneName = farmZone.ZoneName;
            farmZoneEntity.FarmId = farmZone.FarmId;
            farmZoneEntity.IrrigationType = farmZone.IrrigationType;
            farmZoneEntity.SizeInHectares = farmZone.SizeInHectares;
            farmZoneEntity.SoilId = farmZone.SoilId;

            try
            {
                var updatedFarmZone = await _unitOfWork.FarmZones.UpdateAsync(farmZoneEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmZoneDto(updatedFarmZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmZone");
                throw;
            }
        }

        public async Task<FarmZoneDto> DeleteFarmZoneAsync(Guid id)
        {
            var farmZoneEntity = await _unitOfWork.FarmZones.GetByIdAsync(id).ConfigureAwait(false);
            if (farmZoneEntity == null)
                return null;

            try
            {
                var deletedFarmZone = await _unitOfWork.FarmZones.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmZoneDto(new FarmZone());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmZone: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmZoneDto> CreateUpdateFarmZoneBoundaryAsync(FarmZoneDto farmZone)
        {
            try
            {
                var createdFarmZone = new FarmZone();
                var zone = await _unitOfWork.FarmZones.GetByIdAsync(farmZone.ZoneId).ConfigureAwait(false);
                if (zone == null)
                {
                    var newFarmZone = new FarmZone
                    {
                        Notes = farmZone.FarmZoneNotes,
                        ZoneName = farmZone.ZoneName,
                        FarmId = farmZone.FarmId,
                        IrrigationType = farmZone.IrrigationType,
                        SizeInHectares = farmZone.SizeInHectares,
                        ZoneDescription = farmZone.Description,
                        SizeInSqm = farmZone.FarmSizeInSqm,
                        BoundaryJson = farmZone.ZoneBoundaryJson,
                        Lat = farmZone.ZoneCentroidLat,
                        Lng = farmZone.ZoneCentroidLng,
                        SoilId = farmZone.SoilId,
                        ZoneColor = farmZone.ZoneColor,
                        Id = farmZone.ZoneId,
                        FarmCrops = [
                            new() {
                                CropId = farmZone.CropId,
                                FarmId = farmZone.FarmId,
                                UnitId = farmZone.UnitId,
                                ZoneId = farmZone.ZoneId,
                                //PlantingDate = farmZone.PlantingDate,
                                //DefaultRate = farmZone.DefaultRate,
                                //EstimatedHarvestDate = farmZone.EstimatedHarvestDate,
                            }
                        ],
                    };

                    createdFarmZone = await _unitOfWork.FarmZones.AddAsync(newFarmZone).ConfigureAwait(false);
                }
                else
                {
                    zone.ZoneDescription = farmZone.Description;
                    zone.BoundaryJson = farmZone.ZoneBoundaryJson;
                    zone.Lat = farmZone.ZoneCentroidLat;
                    zone.Lng = farmZone.ZoneCentroidLng;
                    zone.ZoneColor = farmZone.ZoneColor;
                    zone.Notes = farmZone.FarmZoneNotes;
                    zone.ZoneName = farmZone.ZoneName;
                    zone.FarmId = farmZone.FarmId;
                    zone.IrrigationType = farmZone.IrrigationType;
                    zone.SizeInHectares = farmZone.SizeInHectares;
                    zone.SoilId = farmZone.SoilId;
                    zone.SizeInSqm = farmZone.FarmSizeInSqm;

                    zone.FarmCrops = [
                        new() {
                            CropId = farmZone.CropId,
                            FarmId = farmZone.FarmId,
                            UnitId = farmZone.UnitId,
                            ZoneId = farmZone.ZoneId,
                            //PlantingDate = farmZone.PlantingDate,
                            //DefaultRate = farmZone.DefaultRate,
                            //EstimatedHarvestDate = farmZone.EstimatedHarvestDate,
                        }
                    ];

                    createdFarmZone = await _unitOfWork.FarmZones.UpdateAsync(zone).ConfigureAwait(false);
                }

                // update farm boundary details
                var farmDetails = await _unitOfWork.Farms.GetByIdAsync(farmZone.FarmId).ConfigureAwait(false);
                if (farmDetails != null)
                {
                    farmDetails.Lat = (decimal?)farmZone.FarmCentroidLat;
                    farmDetails.Lng = (decimal?)farmZone.FarmCentroidLng;
                    farmDetails.BoundaryJson = farmZone.FarmBoundaryJson;
                    farmDetails.Location = farmZone.FarmLocation;
                    farmDetails.SizeInHectares = farmZone.SizeInHectares;
                    farmDetails.SizeInSqm = farmZone.FarmSizeInSqm;
                    //farmDetails.SetupComplete = false;
                    farmDetails.SetupStarted = true;

                    await _unitOfWork.Farms.UpdateAsync(farmDetails).ConfigureAwait(false);
                }
                // commit changes only when all operations are successful
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmZoneDto(createdFarmZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating FarmZone boundary");
                throw;
            }
        }

        public async Task<FarmZoneDto> UpdateFarmZoneBoundaryAsync(FarmZoneDto farmZone)
        {
            var farmZoneEntity = await _unitOfWork.FarmZones.GetByIdAsync(farmZone.ZoneId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Zone not found.");

            farmZoneEntity.Notes = farmZone.FarmZoneNotes;
            farmZoneEntity.ZoneName = farmZone.ZoneName;
            farmZoneEntity.FarmId = farmZone.FarmId;
            farmZoneEntity.IrrigationType = farmZone.IrrigationType;
            farmZoneEntity.SizeInHectares = farmZone.SizeInHectares;
            farmZoneEntity.SoilId = farmZone.SoilId;

            try
            {
                var updatedFarmZone = await _unitOfWork.FarmZones.UpdateAsync(farmZoneEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmZoneDto(updatedFarmZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmZone boundary");
                throw;
            }
        }
    }
}
