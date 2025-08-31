using System.Collections.Generic;
using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class GeneralExpenseReportEndpoints
    {
        private readonly ReportService _reportService;
        private readonly ILogger<GeneralExpenseReportEndpoints> _logger;

        public GeneralExpenseReportEndpoints(ReportService reportService, ILogger<GeneralExpenseReportEndpoints> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IResult> GetFarmGeneralExpensesCurrentYearAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.GetFarmGeneralExpensesCurrentYearAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses for current year for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> GetFarmGeneralExpensesPreviousYearAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.GetFarmGeneralExpensesPreviousYearAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses for previous year for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> GetFarmGeneralExpensesCurrentMonthAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.GetFarmGeneralExpensesCurrentMonthAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses for current month for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> GetFarmGeneralExpensesPreviousMonthAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.GetFarmGeneralExpensesPreviousMonthAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses for previous month for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> GetFarmGeneralExpensesCurrentQuarterAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.GetFarmGeneralExpensesCurrentQuarterAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses for current quarter for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> GetFarmGeneralExpensesPreviousQuarterAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.GetFarmGeneralExpensesPreviousQuarterAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm general expenses for previous quarter for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> Get12MonthExpensesSummaryListAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.Get12MonthExpensesSummaryListAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving 12 month expenses summary for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }

        public async Task<IResult> Get12MonthExpensesSummaryListPreviousAsync(Guid farmId)
        {
            try
            {
                var list = await _reportService.Get12MonthExpensesSummaryListPreviousAsync(farmId).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving previous 12 month expenses summary for farm {FarmId}", farmId);
                return Results.Problem("An error occurred while retrieving the data.");
            }
        }
    }
}
