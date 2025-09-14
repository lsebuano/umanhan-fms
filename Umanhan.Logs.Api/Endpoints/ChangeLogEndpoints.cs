using Umanhan.Services.Interfaces;

namespace Umanhan.Logs.Api.Endpoints
{
    public class ChangeLogEndpoints
    {
        private readonly IChangeLogService _changeLogService;
        private readonly ILogger<ChangeLogEndpoints> _logger;

        public ChangeLogEndpoints(IChangeLogService changeLogService, ILogger<ChangeLogEndpoints> logger)
        {
            _changeLogService = changeLogService;
            _logger = logger;
        }

        public async Task<IResult> GetChangeLogsAsync(DateTime date)
        {
            try
            {
                var logs = await _changeLogService.GetChangeLogsAsync(date).ConfigureAwait(false);
                return Results.Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving change logs");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetChangeLogByIdAsync(Guid id)
        {
            try
            {
                var changeLog = await _changeLogService.GetChangeLogByIdAsync(id).ConfigureAwait(false);
                return changeLog is not null ? Results.Ok(changeLog) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving change log with ID {ChangeLogId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
