using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChangeLogService> _logger;

        public ChangeLogService(IUnitOfWork unitOfWork,
            ILogger<ChangeLogService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        private static List<ChangeLogDto> ToLogDto(IEnumerable<ChangeLog> logs)
        {
            return logs.Select(x => new ChangeLogDto
            {
                ChangeId = x.Id,
                Type = x.Type,
                EntityId = x.EntityId,
                Timestamp = x.Timestamp,
                Entity = x.Entity,
                Field = x.Field,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                Username = x.Username,
            })
            .OrderByDescending(x => x.Timestamp)
            .ToList();
        }

        private static ChangeLogDto ToLogDto(ChangeLog log)
        {
            return new ChangeLogDto
            {
                ChangeId = log.Id,
                Type = log.Type,
                EntityId = log.EntityId,
                Timestamp = log.Timestamp,
                Entity = log.Entity,
                Field = log.Field,
                OldValue = log.OldValue,
                NewValue = log.NewValue,
                Username = log.Username,
            };
        }

        public async Task<IEnumerable<ChangeLogDto>> GetChangeLogsAsync(DateTime date)
        {
            var obj = await _unitOfWork.ChangeLogs.GetLogsAsync(date).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLogDto(obj);
        }

        public async Task<ChangeLogDto> GetChangeLogByIdAsync(Guid id)
        {
            var obj = await _unitOfWork.ChangeLogs.GetByIdAsync(id).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLogDto(obj);
        }
    }
}
