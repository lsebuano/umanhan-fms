using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class UnitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<UnitService> _logger;

        private static List<UnitDto> ToUnitDto(IEnumerable<Unit> units)
        {
            return [.. units.Select(x => new UnitDto
            {
                UnitId = x.Id,
                UnitName = x.UnitName,
                Crops = x.Crops.Select(y => new CropDto{
                    CropId = y.Id,
                    CropName = y.CropName,
                    CropVariety = y.CropVariety,
                })
                .OrderBy(xx => xx.CropName),
                Inventories = x.Inventories.Select(y => new InventoryDto{
                    ItemName = y.ItemName
                })
                .OrderBy(xx => xx.ItemName)
            })
            .OrderBy(x => x.UnitName)];
        }

        private static UnitDto ToUnitDto(Unit unit)
        {
            return new UnitDto
            {
                UnitId = unit.Id,
                UnitName = unit.UnitName,
                Crops = unit.Crops.Select(y => new CropDto
                {
                    CropId = y.Id,
                    CropName = y.CropName,
                    CropVariety = y.CropVariety,
                })
                .OrderBy(xx => xx.CropName),
                Inventories = unit.Inventories.Select(y => new InventoryDto
                {
                    ItemName = y.ItemName
                })
                .OrderBy(xx => xx.ItemName)
            };
        }

        public UnitService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<UnitService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<UnitDto>> GetAllUnitsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Units.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToUnitDto(list);
        }

        public async Task<UnitDto> GetUnitByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Units.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToUnitDto(obj);
        }

        public async Task<UnitDto> CreateUnitAsync(UnitDto unit)
        {
            var newUnit = new Unit
            {
                UnitName = unit.UnitName,
            };

            try
            {
                var createdUnit = await _unitOfWork.Units.AddAsync(newUnit).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToUnitDto(createdUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unit: {UnitName}", unit.UnitName);
                throw;
            }
        }

        public async Task<UnitDto> UpdateUnitAsync(UnitDto unit)
        {
            var unitEntity = await _unitOfWork.Units.GetByIdAsync(unit.UnitId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Unit not found.");
            unitEntity.UnitName = unit.UnitName;

            try
            {
                var updatedUnit = await _unitOfWork.Units.UpdateAsync(unitEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToUnitDto(updatedUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit: {UnitName}", unit.UnitName);
                throw;
            }
        }

        public async Task<UnitDto> DeleteUnitAsync(Guid id)
        {
            var unitEntity = await _unitOfWork.Units.GetByIdAsync(id).ConfigureAwait(false);
            if (unitEntity == null)
                return null;

            try
            {
                var deletedUnit = await _unitOfWork.Units.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToUnitDto(new Unit());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unit with ID: {UnitId}", id);
                throw;
            }
        }
    }
}
