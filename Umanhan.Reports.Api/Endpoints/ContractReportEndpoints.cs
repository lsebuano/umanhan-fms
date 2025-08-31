using System.Collections.Generic;
using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class ContractReportEndpoints
    {
        private readonly ReportService _reportService;
        private readonly ILogger<ContractReportEndpoints> _logger;

        public ContractReportEndpoints(ReportService reportService, ILogger<ContractReportEndpoints> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IResult> GetContractsExpectedRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsExpectedRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expected revenue for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving expected revenue.");
            }
        }

        public async Task<IResult> GetContractsTotalValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsTotalValueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total value for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total value of contracts.");
            }
        }

        public async Task<IResult> GetContractsTotalLostValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsTotalLostValueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total lost value for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total lost value of contracts.");
            }
        }

        public async Task<IResult> GetContractsApproachingHarvestAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsApproachingHarvestAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approaching harvest for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving approaching harvest contracts.");
            }
        }

        public async Task<IResult> GetContractsNewAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsNewAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting new contracts for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving new contracts.");
            }
        }

        public async Task<IResult> GetContractsHarvestedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsHarvestedAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting harvested contracts for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving harvested contracts.");
            }
        }

        public async Task<IResult> GetContractsConfirmedPickeUpsAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsConfirmedPickeUpsAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting picked up contracts for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving picked up contracts.");
            }
        }

        public async Task<IResult> GetContractsPaidAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var list = await _reportService.GetContractsPaidAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paid contracts for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving paid contracts.");
            }
        }
    }
}
