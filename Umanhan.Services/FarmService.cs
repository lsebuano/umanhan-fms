using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmService> _logger;

        private static List<FarmDto> ToFarmDto(IEnumerable<Farm> farms)
        {
            return [.. farms.Select(x => new FarmDto
            {
                FarmId = x.Id,
                BoundaryJson = x.BoundaryJson,
                FarmName = x.FarmName,
                Lat = x.Lat,
                Location = x.Location,
                Lng = x.Lng,
                Notes = x.Notes,
                OwnerName = x.OwnerName,
                SizeInHectares = x.SizeInHectares,
                SizeInSqm = x.SizeInSqm,
                SetupComplete = x.SetupComplete,
                SetupStarted = x.SetupStarted,
                StaticMapUrl = x.StaticMapUrl,
                FullAddress = x.FullAddress,
                Tin = x.Tin,
                ContactEmail = x.ContactEmail,
                ContactPhone = x.ContactPhone,
                FarmContracts = x.FarmContracts.Select(y => new FarmContractDto{
                    CustomerName = y.Customer?.CustomerName,
                    //ContractDateUtc = y.ContractDate,
                    ContractDate = y.ContractDate.ToDateTime(TimeOnly.MinValue),
                    Status = y.Status,
                })
                .OrderBy(xx => xx.CustomerName),
                FarmCrops = x.FarmCrops.Select(y => new FarmCropDto{
                    FarmCropId = y.Id,

                    FarmName = y.Farm?.FarmName,
                    ZoneId = y.ZoneId,
                    ZoneName = y.Zone?.ZoneName,
                    ZoneIrrigationType = y.Zone?.IrrigationType,
                    ZoneSizeInHectares = y.Zone?.SizeInHectares,
                    ZoneSoilType = y.Zone?.Soil?.SoilName,

                    CropId = y.CropId,
                    CropName = y.Crop?.CropName,
                    CropVariety = y.Crop?.CropVariety,
                    UnitId = y.UnitId,
                    DefaultRate = y.DefaultRate,
                    PlantingDateUtc = y.PlantingDate,
                    EstimatedHarvestDateUtc = y.EstimatedHarvestDate,
                })
                .OrderBy(xx => xx.CropName),
                FarmInventories = x.FarmInventories.Select(y => new FarmInventoryDto {
                    InventoryItemName = y.Inventory?.ItemName,
                    InventoryUnit = y.Inventory?.Unit?.UnitName,
                    Quantity = y.Quantity,
                    InventoryCategory = y.Inventory?.Category?.CategoryName,
                    InventoryCategoryGroup = y.Inventory?.Category?.Group,
                    InventoryCategoryGroup2 = y.Inventory?.Category?.Group2,
                    InventoryCategoryConsumptionBehavior = y.Inventory?.Category?.ConsumptionBehavior,
                })
                .OrderBy(xx => xx.InventoryItemName),
                FarmZones = x.FarmZones.Select(y => new FarmZoneDto {
                    IrrigationType = y.IrrigationType,
                    FarmZoneNotes = y.Notes,
                    ZoneName = y.ZoneName,
                    SoilType = y.Soil?.SoilName,
                    AreaInSqm = y.SizeInSqm,
                    SizeInHectares = y.SizeInHectares,
                })
                .OrderBy(xx => xx.ZoneName),
                FarmLivestocks = x.FarmLivestocks.Select(y => new FarmLivestockDto {
                    LivestockId = y.LivestockId,
                    AnimalType = y.Livestock?.AnimalType,
                    PurchaseCost = y.PurchaseCost,
                    PurchaseDateUtc = y.PurchaseDate,
                    Quantity = y.Quantity,
                    ZoneName = y.Zone?.ZoneName,
                    BirthDateUtc = y.BirthDate,
                    Breed = y.Breed
                })
                .OrderBy(xx => xx.AnimalType),
                Staffs = x.Staffs.Select(y => new StaffDto {
                    StaffId = y.Id,
                    ContactInfo = y.ContactInfo,
                    Name = y.Name,
                    HireDateUtc = y.HireDate,
                    Status = y.Status,
                })
                .OrderBy(xx => xx.Name),
            })
            .OrderBy(x => x.FarmName)];
        }

        private static FarmDto ToFarmDto(Farm farm)
        {
            return new FarmDto
            {
                FarmId = farm.Id,
                BoundaryJson = farm.BoundaryJson,
                FarmName = farm.FarmName,
                Lat = farm.Lat,
                Location = farm.Location,
                Lng = farm.Lng,
                Notes = farm.Notes,
                OwnerName = farm.OwnerName,
                SizeInHectares = farm.SizeInHectares,
                SetupComplete = farm.SetupComplete,
                SetupStarted = farm.SetupStarted,
                SizeInSqm = farm.SizeInSqm,
                StaticMapUrl = farm.StaticMapUrl,
                FullAddress = farm.FullAddress,
                Tin = farm.Tin,
                ContactEmail = farm.ContactEmail,
                ContactPhone = farm.ContactPhone,
                FarmZones = farm.FarmZones.Select(x => new FarmZoneDto
                {
                    AreaInSqm = x.SizeInSqm,
                    Description = x.ZoneDescription,
                    IrrigationType = x.IrrigationType,
                    ZoneName = x.ZoneName,
                    ZoneId = x.Id,
                    ZoneColor = x.ZoneColor,
                    ZoneCentroidLng = x.Lng,
                    ZoneCentroidLat = x.Lat,
                    ZoneBoundaryJson = x.BoundaryJson,
                    SoilType = x.Soil?.SoilName,
                    SoilId = x.SoilId,
                    SizeInHectares = x.SizeInHectares,
                    FarmZoneNotes = x.Notes,
                }),
                FarmCrops = farm.FarmCrops.Select(x => new FarmCropDto
                {
                    FarmCropId = x.Id,

                    FarmName = x.Farm?.FarmName,
                    ZoneId = x.ZoneId,
                    ZoneName = x.Zone?.ZoneName,
                    ZoneIrrigationType = x.Zone?.IrrigationType,
                    ZoneSizeInHectares = x.Zone?.SizeInHectares,
                    ZoneSoilType = x.Zone?.Soil?.SoilName,

                    CropId = x.CropId,
                    CropName = x.Crop?.CropName,
                    CropVariety = x.Crop?.CropVariety,
                    UnitId = x.UnitId,
                    DefaultRate = x.DefaultRate,
                    PlantingDateUtc = x.PlantingDate,
                    EstimatedHarvestDateUtc = x.EstimatedHarvestDate,
                })
            };
        }

        public FarmService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmDto>> GetAllFarmsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Farms.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToFarmDto(list);
        }

        public async Task<FarmDto> GetFarmByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Farms.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmDto(obj);
        }

        public async Task<FarmDto> CreateFarmAsync(FarmDto farm)
        {
            var newFarm = new Farm
            {
                BoundaryJson = farm.BoundaryJson,
                FarmName = farm.FarmName,
                Lat = farm.Lat,
                Lng = farm.Lng,
                Location = farm.Location,
                Notes = farm.Notes,
                OwnerName = farm.OwnerName,
                SizeInHectares = farm.SizeInHectares,
                SetupStarted = false,
                StaticMapUrl = farm.StaticMapUrl,
                FullAddress = farm.FullAddress,
                SizeInSqm = farm.SizeInSqm,
                Tin = farm.Tin,
                ContactEmail = farm.ContactEmail,
                ContactPhone = farm.ContactPhone,
            };

            try
            {
                var createdFarm = await _unitOfWork.Farms.AddAsync(newFarm).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmDto(createdFarm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm");
                throw;
            }
        }

        public async Task<FarmDto> UpdateFarmAsync(FarmDto farm)
        {
            var farmEntity = await _unitOfWork.Farms.GetByIdAsync(farm.FarmId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm not found.");
            farmEntity.BoundaryJson = farm.BoundaryJson;
            farmEntity.FarmName = farm.FarmName;
            farmEntity.Lat = farm.Lat;
            farmEntity.Lng = farm.Lng;
            farmEntity.Location = farm.Location;
            farmEntity.Notes = farm.Notes;
            farmEntity.OwnerName = farm.OwnerName;
            farmEntity.SizeInHectares = farm.SizeInHectares;
            farmEntity.SizeInSqm = farm.SizeInSqm;
            farmEntity.SetupStarted = true;
            farmEntity.StaticMapUrl = farm.StaticMapUrl;
            farmEntity.FullAddress = farm.FullAddress;
            farmEntity.Tin = farm.Tin;
            farmEntity.ContactEmail = farm.ContactEmail;
            farmEntity.ContactPhone = farm.ContactPhone;

            try
            {
                var updatedFarm = await _unitOfWork.Farms.UpdateAsync(farmEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmDto(updatedFarm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmDto> DeleteFarmAsync(Guid id)
        {
            var farmEntity = await _unitOfWork.Farms.GetByIdAsync(id).ConfigureAwait(false);
            if (farmEntity == null)
                return null;

            try
            {
                var deletedFarm = await _unitOfWork.Farms.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmDto(new Farm());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmDto> CompleteFarmSetupAsync(FarmSetupDto farm)
        {
            var farmEntity = await _unitOfWork.Farms.GetByIdAsync(farm.FarmId).ConfigureAwait(false);
            if (farmEntity == null)
                return null;

            try
            {
                farmEntity.FarmName = farm.FarmName;
                farmEntity.Location = farm.Location;
                farmEntity.FullAddress = farm.FullAddress;
                farmEntity.StaticMapUrl = farm.StaticMapUrl;
                farmEntity.OwnerName = farm.OwnerName;
                farmEntity.SizeInHectares = farm.SizeInHectares;
                farmEntity.SizeInSqm = farm.SizeInSqm;
                //farmEntity.Notes = farm.Notes;
                //farmEntity.Lat = farm.Lat;
                //farmEntity.Lng = farm.Lng;
                farmEntity.BoundaryJson = farm.BoundaryJson;
                farmEntity.SetupStarted = true;
                farmEntity.SetupComplete = true;
                
                farmEntity.Tin = farm.Tin;
                farmEntity.ContactEmail = farm.ContactEmail;
                farmEntity.ContactPhone = farm.ContactPhone;

                var updatedFarm = await _unitOfWork.Farms.UpdateAsync(farmEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmDto(updatedFarm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing farm setup: {Message}", ex.Message);
                throw;
            }
        }
    }
}
