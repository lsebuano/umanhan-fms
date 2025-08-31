using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class CropService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<CropService> _logger;

        private static List<CropDto> ToCropDto(IEnumerable<Crop> crops)
        {
            return [.. crops.Select(x => new CropDto
            {
                CropId = x.Id,
                CropName = x.CropName,
                CropVariety = x.CropVariety,
                DefaultRatePerUnit = x.DefaultRatePerUnit,
                DefaultUnitId = x.DefaultUnitId,
                DefaultUnit = x.DefaultUnit?.UnitName,
                Notes = x.Notes,
            })
            .OrderBy(x => x.CropName)];
        }

        private static CropDto ToCropDto(Crop crop)
        {
            return new CropDto
            {
                CropId = crop.Id,
                CropName = crop.CropName,
                CropVariety = crop.CropVariety,
                DefaultRatePerUnit = crop.DefaultRatePerUnit,
                DefaultUnitId = crop.DefaultUnitId,
                DefaultUnit = crop.DefaultUnit?.UnitName,
                Notes = crop.Notes,
            };
        }

        public CropService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<CropService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<CropDto>> GetAllCropsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Crops.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToCropDto(list);
        }

        public async Task<CropDto> GetCropByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Crops.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToCropDto(obj);
        }

        public async Task<CropDto> CreateCropAsync(CropDto crop)
        {
            var newCrop = new Crop
            {
                CropName = crop.CropName,
                CropVariety = crop.CropVariety,
                DefaultRatePerUnit = crop.DefaultRatePerUnit,
                DefaultUnitId = crop.DefaultUnitId,
                Notes = crop.Notes,
            };

            try
            {
                var createdCrop = await _unitOfWork.Crops.AddAsync(newCrop).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCropDto(createdCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating crop: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CropDto> UpdateCropAsync(CropDto crop)
        {
            var cropEntity = await _unitOfWork.Crops.GetByIdAsync(crop.CropId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Crop not found.");
            cropEntity.CropName = crop.CropName;
            cropEntity.CropVariety = crop.CropVariety;
            cropEntity.DefaultRatePerUnit = crop.DefaultRatePerUnit;
            cropEntity.DefaultUnitId = crop.DefaultUnitId;

            try
            {
                var updatedCrop = await _unitOfWork.Crops.UpdateAsync(cropEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCropDto(updatedCrop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating crop: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CropDto> DeleteCropAsync(Guid id)
        {
            var cropEntity = await _unitOfWork.Crops.GetByIdAsync(id).ConfigureAwait(false);
            if (cropEntity == null)
                return null;

            try
            {
                var deletedCrop = await _unitOfWork.Crops.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToCropDto(new Crop());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting crop: {Message}", ex.Message);
                throw;
            }
        }
    }
}
