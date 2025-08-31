using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories;
using Umanhan.Repositories.LoggerContext.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class LogService
    {
        private readonly ILoggerUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<LogService> _logger;

        public LogService(ILoggerUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<LogService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        private static List<LogDto> ToLogDto(IEnumerable<Log> logs)
        {
            return logs.Select(x => new LogDto
            {
                LogId = x.Id,
                FarmId = x.FarmId,
                Timestamp = x.Timestamp,
                Level = x.Level,
                Message = x.Message,
                Exception = x.Exception,
                Properties = x.Properties,
            })
            .OrderByDescending(x => x.Timestamp)
            .ToList();
        }

        private static LogDto ToLogDto(Log log)
        {
            return new LogDto
            {
                LogId = log.Id,
                Timestamp = log.Timestamp,
                Level = log.Level,
                Message = log.Message,
                Exception = log.Exception,
                Properties = log.Properties,
            };
        }

        public async Task<object?> GetLogsAsync(Guid farmId, DateTime date)
        {
            var obj = await _unitOfWork.Logs.GetLogsAsync(farmId, date).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLogDto(obj);
        }

        public async Task<LogDto> GetLogByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Logs.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLogDto(obj);
        }

        public async Task<LogDto> CreateLogAsync(LogDto log)
        {
            var newLog = new Log
            {
                Exception = log.Exception,
                Level = log.Level,
                Message = log.Message,
                Properties = log.Properties,
                Timestamp = log.Timestamp,
                FarmId = log.FarmId,
            };

            try
            {
                var createdLog = await _unitOfWork.Logs.AddAsync(newLog).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                return ToLogDto(createdLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Log: {Message}", ex.Message);
                throw;
            }
        }
    }
}
