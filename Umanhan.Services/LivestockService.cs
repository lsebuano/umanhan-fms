using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class LivestockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<LivestockService> _logger;

        private static List<LivestockDto> ToLivestockDto(IEnumerable<Livestock> livestocks)
        {
            return [.. livestocks.Select(x => new LivestockDto
            {
                LivestockId = x.Id,
                AnimalType = x.AnimalType,
                Breed = x.Breed,
            })
            .OrderBy(x => x.AnimalType)];
        }

        private static LivestockDto ToLivestockDto(Livestock livestock)
        {
            return new LivestockDto
            {
                LivestockId = livestock.Id,
                AnimalType = livestock.AnimalType,
                Breed = livestock.Breed,
            };
        }

        public LivestockService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<LivestockService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<LivestockDto>> GetAllLivestocksAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Livestocks.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToLivestockDto(list);
        }

        public async Task<LivestockDto> GetLivestockByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Livestocks.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLivestockDto(obj);
        }

        public async Task<LivestockDto> CreateLivestockAsync(LivestockDto livestock)
        {
            var newLivestock = new Livestock
            {
                AnimalType = livestock.AnimalType,
                Breed = livestock.Breed,
            };

            try
            {
                var createdLivestock = await _unitOfWork.Livestocks.AddAsync(newLivestock).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToLivestockDto(createdLivestock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating livestock");
                throw;
            }
        }

        public async Task<LivestockDto> UpdateLivestockAsync(LivestockDto livestock)
        {
            var livestockEntity = await _unitOfWork.Livestocks.GetByIdAsync(livestock.LivestockId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Livestock not found.");
            livestockEntity.AnimalType = livestock.AnimalType;
            livestockEntity.Breed = livestock.Breed;

            try
            {
                var updatedLivestock = await _unitOfWork.Livestocks.UpdateAsync(livestockEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToLivestockDto(updatedLivestock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating livestock: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<LivestockDto> DeleteLivestockAsync(Guid id)
        {
            var livestockEntity = await _unitOfWork.Livestocks.GetByIdAsync(id).ConfigureAwait(false);
            if (livestockEntity == null)
                return null;

            try
            {
                var deletedLivestock = await _unitOfWork.Livestocks.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToLivestockDto(new Livestock());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting livestock: {Message}", ex.Message);
                throw;
            }
        }
    }
}
