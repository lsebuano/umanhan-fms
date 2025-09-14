using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmActivityPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmActivityPhotoService> _logger;

        private static List<FarmActivityPhotoDto> ToFarmActivityPhotoDto(IEnumerable<FarmActivityPhoto> farmActivityPhotos)
        {
            return [.. farmActivityPhotos.Select(x => new FarmActivityPhotoDto
            {
                PhotoId = x.Id,
                ActivityId = x.ActivityId,
                MimeType = x.MimeType,
                Notes = x.Notes,
                PhotoUrlFull = x.PhotoUrlFull,
                PhotoUrlThumbnail = x.PhotoUrlThumbnail,
                Timestamp = x.Timestamp
            })];
        }

        private static FarmActivityPhotoDto ToFarmActivityPhotoDto(FarmActivityPhoto farmActivityPhoto)
        {
            return new FarmActivityPhotoDto
            {
                PhotoId = farmActivityPhoto.Id,
                ActivityId = farmActivityPhoto.ActivityId,
                MimeType = farmActivityPhoto.MimeType,
                Notes = farmActivityPhoto.Notes,
                PhotoUrlFull = farmActivityPhoto.PhotoUrlFull,
                PhotoUrlThumbnail = farmActivityPhoto.PhotoUrlThumbnail,
                Timestamp = farmActivityPhoto.Timestamp
            };
        }

        public FarmActivityPhotoService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmActivityPhotoService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmActivityPhotoDto>> GetFarmActivityPhotosByActivityAsync(Guid activityId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivityPhotos.GetFarmActivityPhotosByActivityAsync(activityId, includeEntities).ConfigureAwait(false);
            return ToFarmActivityPhotoDto(list);
        }

        public async Task<FarmActivityPhotoDto> GetFarmActivityPhotoByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmActivityPhotos.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmActivityPhotoDto(obj);
        }

        public async Task<FarmActivityPhotoDto> CreateFarmActivityPhotoAsync(FarmActivityPhotoDto farmActivityPhoto)
        {
            var newFarmActivityPhoto = new FarmActivityPhoto
            {
                ActivityId = farmActivityPhoto.ActivityId,
                MimeType = farmActivityPhoto.MimeType,
                Notes = farmActivityPhoto.Notes,
                PhotoUrlFull = farmActivityPhoto.PhotoUrlFull,
                PhotoUrlThumbnail = farmActivityPhoto.PhotoUrlThumbnail,
                Timestamp = DateTime.Now
            };

            try
            {
                // Create the farm activity usage
                var createdFarmActivityPhoto = await _unitOfWork.FarmActivityPhotos.AddAsync(newFarmActivityPhoto).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityPhotoDto(createdFarmActivityPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FarmActivityPhoto: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityPhotoDto> UpdateFarmActivityPhotoAsync(FarmActivityPhotoDto farmActivityPhoto)
        {
            var farmActivityPhotoEntity = await _unitOfWork.FarmActivityPhotos.GetByIdAsync(farmActivityPhoto.PhotoId).ConfigureAwait(false) ?? throw new KeyNotFoundException("FarmActivityPhoto not found.");
            farmActivityPhotoEntity.ActivityId = farmActivityPhoto.ActivityId;
            farmActivityPhotoEntity.MimeType = farmActivityPhoto.MimeType;
            farmActivityPhotoEntity.Notes = farmActivityPhoto.Notes;
            farmActivityPhotoEntity.PhotoUrlFull = farmActivityPhoto.PhotoUrlFull;
            farmActivityPhotoEntity.PhotoUrlThumbnail = farmActivityPhoto.PhotoUrlThumbnail;
            farmActivityPhotoEntity.Timestamp = DateTime.Now;

            try
            {
                var updatedFarmActivityPhoto = await _unitOfWork.FarmActivityPhotos.UpdateAsync(farmActivityPhotoEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityPhotoDto(updatedFarmActivityPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FarmActivityPhoto: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityPhotoDto> DeleteFarmActivityPhotoAsync(Guid id)
        {
            var farmActivityPhotoEntity = await _unitOfWork.FarmActivityPhotos.GetByIdAsync(id).ConfigureAwait(false);
            if (farmActivityPhotoEntity == null)
                return null;

            try
            {
                var deletedFarmActivityPhoto = await _unitOfWork.FarmActivityPhotos.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityPhotoDto(new FarmActivityPhoto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FarmActivityPhoto: {Message}", ex.Message);
                throw;
            }
        }
    }
}
