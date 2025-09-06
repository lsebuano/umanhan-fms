using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ReportService
    {
        private readonly ApiService _apiService;

        public ReportService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ApiResponse<string>> GenerateNlpBasedReportAsync(string prompt)
        {
            try
            {
                return await _apiService.PostAsync<string, string>("ReportAPI", "api/reports/generate-nlp-based-report", prompt).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region DASHBOARD KPIs
        public async Task<decimal> GetCostOfGoodsSoldAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/cost-of-goods-sold/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetGrossMarginPercentAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/gross-margin-percent/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetNetProfitMarginAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/net-profit-margin/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetTotalOperatingExpensesAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/operating-expenses/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetOperatingExpenseRatioAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/operating-expense-ratio/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/total-revenue/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetTotalGrossProfitAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/gross-profit/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetTotalNetProfitAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/net-profit/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<decimal> GetYieldPerHectareAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<decimal>("ReportAPI", $"api/pnl/yield-per-hectare/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }

        public async Task<Dictionary<string, decimal>> GetTotalDonatedAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<Dictionary<string, decimal>>("ReportAPI", $"api/pnl/total-donated/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<Dictionary<string, decimal>> GetTotalSpoilageAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<Dictionary<string, decimal>>("ReportAPI", $"api/pnl/total-spoilage/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetGrossProfitListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/gross-profit/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/net-profit/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/cost-of-goods-sold/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/operating-expenses/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/operating-expense-ratio/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalRevenueListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/total-revenue/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalDonatedListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/total-donated/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalSpoilageListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/total-spoilage/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        //public async Task<IEnumerable<SparklineChartDto>> GetGrossMarginPercentListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/gross-margin-percent/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
        //        if (!response.IsSuccess)
        //        {
        //            if (response.Errors != null && response.Errors.Any())
        //            {
        //                var errors = new Dictionary<string, List<string>>();
        //                foreach (var error in response.Errors)
        //                {
        //                    errors.Add(error.Key, error.Value);
        //                }
        //                response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
        //            }
        //        }
        //        return response.Data;
        //    }
        //    catch (Exception ex)
        //    {
        //        return default!;
        //    }
        //}

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitMarginListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/net-profit-margin/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        //public async Task<IEnumerable<SparklineChartDto>> GetYieldPerHectareListAsync(Guid farmId, DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/pnl/yield-per-hectare/list/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
        //        if (!response.IsSuccess)
        //        {
        //            if (response.Errors != null && response.Errors.Any())
        //            {
        //                var errors = new Dictionary<string, List<string>>();
        //                foreach (var error in response.Errors)
        //                {
        //                    errors.Add(error.Key, error.Value);
        //                }
        //                response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
        //            }
        //        }
        //        return response.Data;
        //    }
        //    catch (Exception ex)
        //    {
        //        return default!;
        //    }
        //}
        #endregion

        #region CONTRACTS KPIs
        public async Task<ApiResponse<List<FarmContractDetailDto>>> GetContractsExpectedRevenueAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDetailDto>>("ReportAPI", $"api/contracts/expected-revenue/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDetailDto>>> GetContractsTotalValueAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDetailDto>>("ReportAPI", $"api/contracts/total-value/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDetailDto>>> GetContractsTotalLostValueAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDetailDto>>("ReportAPI", $"api/contracts/total-value-lost/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDetailDto>>> GetContractsApproachingHarvestAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDetailDto>>("ReportAPI", $"api/contracts/approaching-harvest/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDto>>> GetContractsNewAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDto>>("ReportAPI", $"api/contracts/new/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDetailDto>>> GetContractsHarvestedAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDetailDto>>("ReportAPI", $"api/contracts/harvested/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDto>>> GetContractsConfirmedPickeUpsAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDto>>("ReportAPI", $"api/contracts/pickedup/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<FarmContractDto>>> GetContractsPaidAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractDto>>("ReportAPI", $"api/contracts/paid/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<FarmContractPaymentDto>> GenerateReceiptAsync(PaymentDetailsDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return new ApiResponse<FarmContractPaymentDto>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Data cannot be null."
                    };
                }
                return await _apiService.PostAsync<PaymentDetailsDto, FarmContractPaymentDto>("ReportAPI", "api/receipts/generate", dto).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }
        #endregion

        #region SALES KPIs
        public async Task<ApiResponse<List<FarmContractSaleDto>>> GetFarmSalesAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<List<FarmContractSaleDto>>("ReportAPI", $"api/sales/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<MonthlySalesDto>>> GetMonthlySalesAsync(Guid farmId, int year)
        {
            try
            {
                return await _apiService.GetAsync<List<MonthlySalesDto>>("ReportAPI", $"api/sales/monthly-sales/{farmId}/{year}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<List<MonthlySalesDto>>> GetMonthlySalesByCustomerAsync(Guid farmId, int year)
        {
            try
            {
                return await _apiService.GetAsync<List<MonthlySalesDto>>("ReportAPI", $"api/sales/monthly-sales-by-customer/{farmId}/{year}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }
        #endregion

        #region GENERAL EXPENSES KPIs
        public async Task<ApiResponse<KeyValuePair<decimal, string>>> GetFarmGeneralExpensesCurrentYearAsync(Guid farmId)
        {
            try
            {
                return await _apiService.GetAsync<KeyValuePair<decimal, string>>("ReportAPI", $"api/gen-expenses/current-year/{farmId}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<KeyValuePair<decimal, string>>> GetFarmGeneralExpensesPreviousYearAsync(Guid farmId)
        {
            try
            {
                return await _apiService.GetAsync<KeyValuePair<decimal, string>>("ReportAPI", $"api/gen-expenses/previous-year/{farmId}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<KeyValuePair<decimal, string>>> GetFarmGeneralExpensesCurrentMonthAsync(Guid farmId)
        {
            try
            {
                return await _apiService.GetAsync<KeyValuePair<decimal, string>>("ReportAPI", $"api/gen-expenses/current-month/{farmId}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<KeyValuePair<decimal, string>>> GetFarmGeneralExpensesPreviousMonthAsync(Guid farmId)
        {
            try
            {
                return await _apiService.GetAsync<KeyValuePair<decimal, string>>("ReportAPI", $"api/gen-expenses/previous-month/{farmId}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<KeyValuePair<decimal, string>>> GetFarmGeneralExpensesCurrentQuarterAsync(Guid farmId)
        {
            try
            {
                return await _apiService.GetAsync<KeyValuePair<decimal, string>>("ReportAPI", $"api/gen-expenses/current-quarter/{farmId}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<ApiResponse<KeyValuePair<decimal, string>>> GetFarmGeneralExpensesPreviousQuarterAsync(Guid farmId)
        {
            try
            {
                return await _apiService.GetAsync<KeyValuePair<decimal, string>>("ReportAPI", $"api/gen-expenses/previous-quarter/{farmId}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListAsync(Guid farmId)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/gen-expenses/trend-list-12-months/{farmId}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryPreviousListAsync(Guid farmId)
        {
            try
            {
                var response = await _apiService.GetAsync<IEnumerable<SparklineChartDto>>("ReportAPI", $"api/gen-expenses/trend-list-12-months/{farmId}/previous").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                return default!;
            }
        }
        #endregion

        #region PRODUCT PERFORMANCE KPIs
        public async Task<ApiResponse<IEnumerable<ProductPerformanceDto>>> GetProductPerformancesAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _apiService.GetAsync<IEnumerable<ProductPerformanceDto>>("ReportAPI", $"api/product-performances/{farmId}/{startDate:O}/{endDate:O}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return default!;
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductPerformanceDto>>> GetProductPerformancesByYearAsync(Guid farmId, int year)
        {
            try
            {
                return await _apiService.GetAsync<IEnumerable<ProductPerformanceDto>>("ReportAPI", $"api/product-performances/year/{farmId}/{year}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return default!;
            }
        }


        #endregion
    }
}
