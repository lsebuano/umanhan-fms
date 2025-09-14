using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.LoggerContext.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class QueryLogService : IQueryLogService
    {
        private readonly ILoggerUnitOfWork _unitOfWork;
        private readonly ILogger<QueryLogService> _logger;

        public QueryLogService(ILoggerUnitOfWork unitOfWork,
            ILogger<QueryLogService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        private static List<QueryLogDto> ToLogDto(IEnumerable<EfQueryLog> logs)
        {
            return logs.Select(x => new QueryLogDto
            {
                QueryId = x.Id,
                ApiEndpoint = x.ApiEndpoint,
                CorrelationId = x.CorrelationId,
                CreatedAt = x.CreatedAt,
                DurationMs = x.DurationMs,
                Environment = x.Environment,
                FarmId = x.FarmId,
                HttpMethod = x.HttpMethod,
                Parameters = x.Parameters,
                Query = x.Query,
                RowsReturned = x.RowsReturned,
                Source = x.Source,
                UserId = x.UserId,
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
        }

        private static QueryLogDto ToLogDto(EfQueryLog log)
        {
            return new QueryLogDto
            {
                QueryId = log.Id,
                ApiEndpoint = log.ApiEndpoint,
                CorrelationId = log.CorrelationId,
                CreatedAt = log.CreatedAt,
                DurationMs = log.DurationMs,
                Environment = log.Environment,
                FarmId = log.FarmId,
                HttpMethod = log.HttpMethod,
                Parameters = log.Parameters,
                Query = log.Query,
                RowsReturned = log.RowsReturned,
                Source = log.Source,
                UserId = log.UserId,
            };
        }

        public async Task<IEnumerable<QueryLogDto>> GetQueryLogsAsync(DateTime date)
        {
            var obj = await _unitOfWork.QueryLogs.GetLogsAsync(date).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLogDto(obj);
        }

        public async Task<QueryLogDto> GetQueryLogByIdAsync(Guid id)
        {
            var obj = await _unitOfWork.QueryLogs.GetByIdAsync(id).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToLogDto(obj);
        }
    }
}
