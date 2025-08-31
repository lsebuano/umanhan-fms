using System.Collections.Generic;
using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class SalesReportEndpoints
    {
        private readonly ReportService _reportService;
        private readonly ILogger<SalesReportEndpoints> _logger;

        public SalesReportEndpoints(ReportService reportService, ILogger<SalesReportEndpoints> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IResult> GetFarmSalesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetFarmSalesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales revenue for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving sales revenue.");
            }
        }

        public async Task<IResult> GetMonthlySalesAsync(Guid farmId, int year)
        {
            try
            {
                var list = await _reportService.GetMonthlySalesAsync(farmId, year).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly sales for farm {FarmId} in year {Year}", farmId, year);
                return Results.Problem("An error occurred while retrieving monthly sales.");
            }
        }

        public async Task<IResult> GetMonthlySalesByCustomerAsync(Guid farmId, int year)
        {
            try
            {
                var list = await _reportService.GetMonthlySalesByCustomerAsync(farmId, year).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly sales by customer for farm {FarmId} in year {Year}", farmId, year);
                return Results.Problem("An error occurred while retrieving monthly sales by customer.");
            }
        }
    }
}
