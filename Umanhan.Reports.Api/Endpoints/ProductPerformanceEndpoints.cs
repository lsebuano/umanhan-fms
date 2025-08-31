using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class ProductPerformanceEndpoints
    {
        private readonly ReportService _reportService;
        private readonly ILogger<ProductPerformanceEndpoints> _logger;

        public ProductPerformanceEndpoints(ReportService reportService, ILogger<ProductPerformanceEndpoints> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IResult> GetProductPerformancesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetProductPerformancesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product performance for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving product performance.");
            }
        }

        public async Task<IResult> GetProductPerformancesByYearAsync(Guid farmId, int year)
        {
            try
            {
                var list = await _reportService.GetProductPerformancesByYearAsync(farmId, year).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product performance for farm {FarmId} for year {Year}", farmId, year);
                return Results.Problem("An error occurred while retrieving product performance by year.");
            }
        }
    }
}
