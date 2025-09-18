using Umanhan.Services.Interfaces;

namespace Umanhan.Logs.Api.Endpoints
{
    public class QueryLogEndpoints
    {
        private readonly IQueryLogService _queryLogService;
        private readonly ILogger<QueryLogEndpoints> _logger;

        public QueryLogEndpoints(IQueryLogService queryLogService, ILogger<QueryLogEndpoints> logger)
        {
            _queryLogService = queryLogService;
            _logger = logger;
        }

        public async Task<IResult> GetQueryLogsAsync(DateTime date, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var logs = await _queryLogService.GetQueryLogsAsync(date, pageNumber, pageSize).ConfigureAwait(false);
                return Results.Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving query logs");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetQueryLogByIdAsync(Guid id)
        {
            try
            {
                var queryLog = await _queryLogService.GetQueryLogByIdAsync(id).ConfigureAwait(false);
                return queryLog is not null ? Results.Ok(queryLog) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving query log with ID {QueryLogId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
