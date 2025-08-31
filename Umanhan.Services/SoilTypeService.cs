using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class SoilTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<SoilTypeService> _logger;

        private static List<SoilTypeDto> ToSoilTypeDto(IEnumerable<SoilType> soilTypes)
        {
            return [.. soilTypes.Select(x => new SoilTypeDto
            {
                SoilId = x.Id,
                SoilFertility = x.SoilFertility,
                SoilMoisture = x.SoilMoisture,
                SoilName = x.SoilName,
                SoilOrganicCarbon = x.SoilOrganicCarbon,
                SoilPh = x.SoilPh,
                Notes = x.Notes,
            })
            .OrderBy(x => x.SoilName)];
        }

        private static SoilTypeDto ToSoilTypeDto(SoilType soilType)
        {
            return new SoilTypeDto
            {
                SoilId = soilType.Id,
                SoilFertility = soilType.SoilFertility,
                SoilMoisture = soilType.SoilMoisture,
                SoilName = soilType.SoilName,
                SoilOrganicCarbon = soilType.SoilOrganicCarbon,
                SoilPh = soilType.SoilPh,
                Notes = soilType.Notes,
            };
        }

        public SoilTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<SoilTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<SoilTypeDto>> GetAllSoilTypesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.SoilTypes.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToSoilTypeDto(list);
        }

        public async Task<SoilTypeDto> GetSoilTypeByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.SoilTypes.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToSoilTypeDto(obj);
        }

        public async Task<SoilTypeDto> CreateSoilTypeAsync(SoilTypeDto soilType)
        {
            var newSoilType = new SoilType
            {
                SoilFertility = soilType.SoilFertility,
                SoilMoisture = soilType.SoilMoisture,
                SoilName = soilType.SoilName,
                SoilOrganicCarbon = soilType.SoilOrganicCarbon,
                SoilPh = soilType.SoilPh,
                Notes = soilType.Notes,
            };

            try
            {
                var createdSoilType = await _unitOfWork.SoilTypes.AddAsync(newSoilType).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToSoilTypeDto(createdSoilType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SoilType");
                throw;
            }
        }

        public async Task<SoilTypeDto> UpdateSoilTypeAsync(SoilTypeDto soilType)
        {
            var soilTypeEntity = await _unitOfWork.SoilTypes.GetByIdAsync(soilType.SoilId).ConfigureAwait(false) ?? throw new KeyNotFoundException("SoilType not found.");
            soilTypeEntity.SoilFertility = soilType.SoilFertility;
            soilTypeEntity.SoilMoisture = soilType.SoilMoisture;
            soilTypeEntity.SoilName = soilType.SoilName;
            soilTypeEntity.SoilOrganicCarbon = soilType.SoilOrganicCarbon;
            soilTypeEntity.SoilPh = soilType.SoilPh;
            soilTypeEntity.Notes = soilType.Notes;

            try
            {
                var updatedSoilType = await _unitOfWork.SoilTypes.UpdateAsync(soilTypeEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToSoilTypeDto(updatedSoilType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SoilType");
                throw;
            }
        }

        public async Task<SoilTypeDto> DeleteSoilTypeAsync(Guid id)
        {
            var soilTypeEntity = await _unitOfWork.SoilTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (soilTypeEntity == null)
                return null;

            try
            {
                var deletedSoilType = await _unitOfWork.SoilTypes.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToSoilTypeDto(new SoilType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting SoilType: {Message}", ex.Message);
                throw;
            }
        }
    }
}
