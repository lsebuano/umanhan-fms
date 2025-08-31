using Azure;
using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class PnlEndpoints
    {
        private readonly ReportService _reportService;
        private readonly ILogger<ReportEndpoints> _logger;

        public PnlEndpoints(ReportService reportService, ILogger<ReportEndpoints> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        #region Dashboard

        public async Task<IResult> GetTotalRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total revenue.");
            }
        }

        public async Task<IResult> GetCostOfGoodsSoldAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting COGS for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving cost of goods sold.");
            }
        }

        public async Task<IResult> GetGrossMarginPercentAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetGrossMarginPercentAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gross margin percent for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving gross margin percent.");
            }
        }

        public async Task<IResult> GetOperatingExpensesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetOperatingExpensesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expenses for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving operating expenses.");
            }
        }

        public async Task<IResult> GetOperatingExpenseRatioAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetOperatingExpenseRatioAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expense ratio for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving operating expense ratio.");
            }
        }

        public async Task<IResult> GetNetProfitMarginAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetNetProfitMarginAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit margin for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving net profit margin.");
            }
        }

        public async Task<IResult> GetGrossProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetGrossProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gross profit for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving gross profit.");
            }
        }

        public async Task<IResult> GetNetProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetNetProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving net profit.");
            }
        }

        public async Task<IResult> GetYieldPerHectareAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetYieldPerHectareAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting yield per hectare for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving yield per hectare.");
            }
        }

        public async Task<IResult> GetTotalDonatedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetTotalDonatedAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total donated produce for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total donated produce.");
            }
        }

        public async Task<IResult> GetTotalSpoilageAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetTotalSpoilageAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total spoiled produce for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total spoiled produce.");
            }
        }


        public async Task<IResult> GetTotalRevenueListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetTotalRevenueListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total revenue list.");
            }
        }

        public async Task<IResult> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetCostOfGoodsSoldListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting COGS for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving cost of goods sold list.");
            }
        }

        public async Task<IResult> GetOperatingExpenseListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetOperatingExpenseListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expenses for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving operating expenses list.");
            }
        }

        public async Task<IResult> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetOperatingExpenseRatioListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expense ratio for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving operating expense ratio list.");
            }
        }

        public async Task<IResult> GetTotalDonatedListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetTotalDonatedListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total donated produce list for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total donated produce list.");
            }
        }

        public async Task<IResult> GetTotalSpoilageListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetTotalSpoilageListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total spoiled produce list for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving total spoiled produce list.");
            }
        }

        public async Task<IResult> GetGrossProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetGrossProfitListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gross profit for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving gross profit list.");
            }
        }

        public async Task<IResult> GetNetProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetNetProfitListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving net profit list.");
            }
        }

        public async Task<IResult> GetNetProfitMarginListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var value = await _reportService.GetNetProfitMarginListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return Results.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit margin for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
                return Results.Problem("An error occurred while retrieving net profit margin list.");
            }
        }

        //public async Task<IResult> GetGrossMarginPercentListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        //{
        //    try
        //    {
        //        var value = await _reportService.GetGrossMarginPercentListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
        //        return Results.Ok(value);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting gross margin percent for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
        //        return Results.Problem("An error occurred while retrieving gross margin percent list.");
        //    }
        //}

        //public async Task<IResult> GetYieldPerHectareListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        //{
        //    try
        //    {
        //        var value = await _reportService.GetYieldPerHectareListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
        //        return Results.Ok(value);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting yield per hectare for farm {FarmId} from {DateStart} to {DateEnd}", farmId, dateStart, dateEnd);
        //        return Results.Problem("An error occurred while retrieving yield per hectare list.");
        //    }
        //}

        #endregion
    }
}
