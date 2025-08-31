using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class SystemSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<SystemSettingService> _logger;

        private static List<SystemSettingDto> ToSystemSettingDto(IEnumerable<SystemSetting> systemSettings)
        {
            return [.. systemSettings.Select(x => new SystemSettingDto
            {
                SettingId = x.Id,
                SettingName = x.SettingName,
                SettingValue = x.SettingValue,
            })
            .OrderBy(x => x.SettingName)];
        }

        private static SystemSettingDto ToSystemSettingDto(SystemSetting systemSetting)
        {
            return new SystemSettingDto
            {
                SettingId = systemSetting.Id,
                SettingName = systemSetting.SettingName,
                SettingValue = systemSetting.SettingValue,
            };
        }

        public SystemSettingService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<SystemSettingService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<SystemSettingDto>> GetAllSystemSettingsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.SystemSettings.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToSystemSettingDto(list);
        }

        public async Task<SystemSettingDto> GetSystemSettingByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.SystemSettings.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToSystemSettingDto(obj);
        }

        public async Task<SystemSettingDto> GetSystemSettingByNameAsync(string name)
        {
            var obj = await _unitOfWork.SystemSettings.GetSystemSettingByName(name).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToSystemSettingDto(obj);
        }

        public async Task<SystemSettingDto> CreateSystemSettingAsync(SystemSettingDto systemSetting)
        {
            var newSystemSetting = new SystemSetting
            {
                SettingName = systemSetting.SettingName,
            };

            try
            {
                var createdSystemSetting = await _unitOfWork.SystemSettings.AddAsync(newSystemSetting).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToSystemSettingDto(createdSystemSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating system setting: {SettingName}", systemSetting.SettingName);
                throw;
            }
        }

        public async Task<SystemSettingDto> UpdateSystemSettingAsync(SystemSettingDto systemSetting)
        {
            var systemSettingEntity = await _unitOfWork.SystemSettings.GetByIdAsync(systemSetting.SettingId).ConfigureAwait(false) ?? throw new KeyNotFoundException("SystemSetting not found.");
            systemSettingEntity.SettingName = systemSetting.SettingName;

            try
            {
                var updatedSystemSetting = await _unitOfWork.SystemSettings.UpdateAsync(systemSettingEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToSystemSettingDto(updatedSystemSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system setting: {SettingName}", systemSetting.SettingName);
                throw;
            }
        }

        public async Task<SystemSettingDto> DeleteSystemSettingAsync(Guid id)
        {
            var systemSettingEntity = await _unitOfWork.SystemSettings.GetByIdAsync(id).ConfigureAwait(false);
            if (systemSettingEntity == null)
                return null;

            try
            {
                var deletedSystemSetting = await _unitOfWork.SystemSettings.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToSystemSettingDto(new SystemSetting());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting system setting: {SettingId}", id);
                throw;
            }
        }
    }
}
