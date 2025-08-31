using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class LaborerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<LaborerService> _logger;

        private static List<LaborerDto> ToLaborerDto(IEnumerable<Laborer> laborers)
        {
            return [.. laborers.Select(x => new LaborerDto
            {
                LaborerId = x.Id,
                Skillset = x.Skillset,
                DailyRate = x.DailyRate,
                ContactInfo = x.ContactInfo,
                ContractedRate = x.ContractedRate,
                Name = $"{x.Name} ({(x.DailyRate??0).ToString("n2")})"
            })
            .OrderBy(x => x.Name)];
        }

        private static LaborerDto ToLaborerDto(Laborer laborer)
        {
            return new LaborerDto
            {
                LaborerId = laborer.Id,
                Skillset = laborer.Skillset,
                DailyRate = laborer.DailyRate,
                ContactInfo = laborer.ContactInfo,
                ContractedRate = laborer.ContractedRate,
                Name = $"{laborer.Name} ({(laborer.DailyRate ?? 0).ToString("n2")})"
            };
        }

        public LaborerService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<LaborerService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<LaborerDto>> GetAllLaborersAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Laborers.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToLaborerDto(list);
        }

        public async Task<LaborerDto> GetLaborerByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Laborers.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLaborerDto(obj);
        }

        public async Task<LaborerDto> CreateLaborerAsync(LaborerDto laborer)
        {
            var newLaborer = new Laborer
            {
                Skillset = laborer.Skillset,
                DailyRate = laborer.DailyRate,
                ContactInfo = laborer.ContactInfo,
                ContractedRate = laborer.ContractedRate,
                Name = laborer.Name,
            };

            try
            {
                var createdLaborer = await _unitOfWork.Laborers.AddAsync(newLaborer).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToLaborerDto(createdLaborer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating laborer");
                throw;
            }
        }

        public async Task<LaborerDto> UpdateLaborerAsync(LaborerDto laborer)
        {
            var laborerEntity = await _unitOfWork.Laborers.GetByIdAsync(laborer.LaborerId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Laborer not found.");
            laborerEntity.Skillset = laborer.Skillset;
            laborerEntity.DailyRate = laborer.DailyRate;
            laborerEntity.ContactInfo = laborer.ContactInfo;
            laborerEntity.ContractedRate = laborer.ContractedRate;
            laborerEntity.Name = laborer.Name;

            try
            {
                var updatedLaborer = await _unitOfWork.Laborers.UpdateAsync(laborerEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToLaborerDto(updatedLaborer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating laborer: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<LaborerDto> DeleteLaborerAsync(Guid id)
        {
            var laborerEntity = await _unitOfWork.Laborers.GetByIdAsync(id).ConfigureAwait(false);
            if (laborerEntity == null)
                return null;

            try
            {
                var deletedLaborer = await _unitOfWork.Laborers.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToLaborerDto(new Laborer());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting laborer: {Message}", ex.Message);
                throw;
            }
        }
    }
}
