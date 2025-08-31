using Azure;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;
using Umanhan.Shared.Extensions;

namespace Umanhan.Services
{
    public class ReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<ReportService> _logger;

        private int GetCurrentQuarter(int month) => (month - 1) / 3 + 1;

        private static string CleanResponse(string raw)
        {
            var trimmed = raw.Trim();

            // Remove leading and trailing quotes if they exist
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                trimmed = trimmed.Substring(1, trimmed.Length - 2);

            // Unescape any \n characters into actual newlines
            trimmed = trimmed.Replace("\\n", "\n");

            return trimmed;
        }

        private static string ExtractSqlStatement(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return string.Empty;

            var trimmed = response.Trim();

            // Remove triple backtick block ```sql ... ```
            if (trimmed.StartsWith("```"))
            {
                var lines = trimmed.Split('\n');
                var contentLines = lines
                    .Skip(1) // skip ```sql or ```
                    .TakeWhile(line => !line.Trim().StartsWith("```"))
                    .ToList();

                trimmed = string.Join("\n", contentLines).Trim();
            }

            // Remove inline backticks `...`
            if (trimmed.StartsWith("`") && trimmed.EndsWith("`"))
            {
                trimmed = trimmed[1..^1].Trim();
            }

            // Remove wrapping quotes
            if ((trimmed.StartsWith("\"") && trimmed.EndsWith("\"")) ||
                (trimmed.StartsWith("'") && trimmed.EndsWith("'")))
            {
                trimmed = trimmed[1..^1].Trim();
            }

            // Optional: Remove trailing semicolon
            if (trimmed.EndsWith(";"))
                trimmed = trimmed[..^1];

            return trimmed;
        }

        private static string MaskSqlStatements(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            // A simple pattern to detect common SQL statements (case-insensitive)
            string sqlPattern = @"(?i)\b(SELECT|INSERT|UPDATE|DELETE|DROP|ALTER|CREATE|EXEC(UTE)?|MERGE|TRUNCATE|GRANT|REVOKE|DECLARE|WITH)\b.*?(;|$)";

            return Regex.Replace(input, sqlPattern, "[SQL_STATEMENT_MASKED]", RegexOptions.Singleline);
        }

        private async Task<List<FarmContractDto>> ToFarmContractDto(IEnumerable<FarmContract> farmContracts)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return [.. farmContracts.Select(x => new FarmContractDto
            {
                ContractDate = x.ContractDate.ToDateTime(TimeOnly.MinValue),
                ContractId = x.Id,
                CustomerId = x.CustomerId,
                FarmId = x.FarmId,
                Status = x.Status,
                CustomerAddress = x.Customer?.Address,
                CustomerContactInfo = x.Customer?.ContactInfo,
                CustomerName = x.Customer?.CustomerName,
                FarmName = x.Farm?.FarmName,
                FarmLocation = x.Farm?.Location,
                FarmContractDetails = x.FarmContractDetails.Select(z=> new FarmContractDetailDto {
                    ContractDetailId = z.Id,
                    ContractId = z.ContractId,
                    ContractedQuantity = z.ContractedQuantity,
                    DeliveredQuantity = z.DeliveredQuantity,
                    ContractedUnitPrice = z.ContractedUnitPrice,
                    ProductId = z.ProductId,
                    Product = productLookup[new ProductKey(z.ProductId, z.ProductTypeId)].ProductName,
                    ProductTypeId = z.ProductTypeId,
                    ProductType = z.ProductType?.ProductTypeName,
                    Unit = z.Unit?.UnitName,
                    UnitId = z.UnitId,
                    Status = z.Status,
                    HarvestDate = z.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                    PickupDate = z.PickupDate?.ToDateTime(TimeOnly.MinValue),
                    PickupConfirmed = z.PickupConfirmed,
                    PaidDate = z.PaidDate?.ToDateTime(TimeOnly.MinValue),
                    PricingProfileId = z.PricingProfileId,
                })
            })];
        }

        private async Task<List<FarmContractDetailDto>> ToFarmContractDetailDto(IEnumerable<FarmContractDetail> farmContractDetails)
        {
            var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
            return [.. farmContractDetails.Select(z=> new FarmContractDetailDto {
                    ContractDetailId = z.Id,
                    ContractId = z.ContractId,
                    ContractedQuantity = z.ContractedQuantity,
                    DeliveredQuantity = z.DeliveredQuantity,
                    ContractedUnitPrice = z.ContractedUnitPrice,
                    ProductId = z.ProductId,
                    Product = productLookup[new ProductKey(z.ProductId, z.ProductTypeId)].ProductName,
                    ProductTypeId = z.ProductTypeId,
                    ProductType = z.ProductType?.ProductTypeName,
                    Unit = z.Unit?.UnitName,
                    UnitId = z.UnitId,
                    Status = z.Status,
                    HarvestDate = z.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                    PickupDate = z.PickupDate?.ToDateTime(TimeOnly.MinValue),
                    PickupConfirmed = z.PickupConfirmed,
                    PaidDate = z.PaidDate?.ToDateTime(TimeOnly.MinValue),
                    PricingProfileId = z.PricingProfileId,
            })];
        }

        private IEnumerable<FarmContractSaleDto> ToFarmContractSaleDto(IEnumerable<FarmContractSale> list)
        {
            return [.. list.Select(z => new FarmContractSaleDto {
                ContractSaleId = z.Id,
                ContractDetailId = z.ContractDetailId,
                UnitId = z.UnitId,
                ProductId = z.ProductId,
                CustomerId = z.CustomerId,
                ProductTypeId = z.ProductTypeId,
                Product = z.ProductName,
                ProductVariety = z.ProductVariety,
                ProductType = z.ProductTypeName,
                Customer = z.CustomerName,
                Unit = z.UnitName,
                Quantity = z.Quantity,
                UnitPrice = z.UnitPrice,
                TotalAmount = z.TotalAmount,
                Date = z.Date.ToDateTime(TimeOnly.MinValue),
                Notes = z.Notes
            })];
        }

        public ReportService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<ReportService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<KeyValuePair<bool, IEnumerable<dynamic>>> RunSqlAsync(string sql, CancellationToken ct = default)
        {
            sql = ExtractSqlStatement(CleanResponse(sql));

            if (string.IsNullOrEmpty(sql))
            {
                throw new InvalidOperationException("Failed: AI must return a SQL. Last output: empty");
            }

            var trimmed = sql.TrimStart();
            if (!trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) &&
                !trimmed.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
            {
                sql = MaskSqlStatements(sql);
                return new KeyValuePair<bool, IEnumerable<dynamic>>(false, [sql]);
            }

            if (trimmed.Length > 5000)
            {
                sql = MaskSqlStatements(sql);
                throw new InvalidOperationException($"Failed: AI must return a shorter response. Last output: {sql}");
            }

            try
            {
                await _unitOfWork.Reports.ExecuteReportQueryAsync($"EXPLAIN {sql}", ct).ConfigureAwait(false);

                var result = await _unitOfWork.Reports.ExecuteReportQueryAsync(sql, ct).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);
                return new KeyValuePair<bool, IEnumerable<dynamic>>(true, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running SQL: {Sql}", sql);
                throw new InvalidOperationException($"Could not run SQL.");
            }

            // should never reach here
            throw new InvalidOperationException("Unexpected error in RunAndGenerateAsync");
        }

        #region DASHBOARD KPIs
        public async Task<decimal> GetTotalRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetCostOfGoodsSoldAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting COGS for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetGrossMarginPercentAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetGrossMarginPercentAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetGrossMarginPercentAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gross margin percent for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetOperatingExpenseRatioAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetOperatingExpenseRatioAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetOperatingExpenseRatioAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expense ratio for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetOperatingExpensesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetOperatingExpensesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetOperatingExpensesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expenses for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetNetProfitMarginAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetNetProfitMarginAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetNetProfitMarginAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit margin for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetGrossProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetGrossProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetGrossProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gross profit for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetNetProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                //var value = await _unitOfWork.Reports.GetNetProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetNetProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<decimal> GetYieldPerHectareAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return 0;

            try
            {
                var value = await _unitOfWork.Reports.GetYieldPerHectareAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting yield per hectare for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return 0;
            }
        }

        public async Task<Dictionary<string, decimal>> GetTotalDonatedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var value = await _unitOfWork.Reports.GetTotalDonatedAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total donated for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<Dictionary<string, decimal>> GetTotalSpoilageAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var value = await _unitOfWork.Reports.GetTotalSpoilageAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total spoilage for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }


        public async Task<IEnumerable<SparklineChartDto>> GetTotalRevenueListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default!;

            try
            {
                //var value = await _unitOfWork.Reports.GetTotalRevenueListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetTotalRevenueListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default!;

            try
            {
                //var value = await _unitOfWork.Reports.GetCostOfGoodsSoldListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetCostOfGoodsSoldListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting COGS for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default!;

            try
            {
                //var value = await _unitOfWork.Reports.GetOperatingExpenseRatioListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetOperatingExpenseRatioListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expense ratio for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default!;

            try
            {
                //var value = await _unitOfWork.Reports.GetOperatingExpenseListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetOperatingExpenseListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operating expenses for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalDonatedListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default!;

            try
            {
                //var value = await _unitOfWork.Reports.GetTotalDonatedListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetTotalDonatedListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total donated for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default!;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalSpoilageListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default!;

            try
            {
                //var value = await _unitOfWork.Reports.GetTotalSpoilageListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                var value = await _unitOfWork.FarmKpiSummaries.GetTotalSpoilageListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total spoilage for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default!;
            }
        }
        #endregion

        #region CONTRACTS KPIs
        public async Task<List<FarmContractDetailDto>> GetContractsExpectedRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsExpectedRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDetailDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expected revenue for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<FarmContractDetailDto>> GetContractsTotalValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsTotalValueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDetailDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total value for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<List<FarmContractDetailDto>> GetContractsTotalLostValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsTotalLostValueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDetailDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total lost value for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<FarmContractDetailDto>> GetContractsApproachingHarvestAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsApproachingHarvestAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDetailDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approaching harvest contracts for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<FarmContractDto>> GetContractsNewAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsNewAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting new contracts for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<FarmContractDetailDto>> GetContractsHarvestedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsHarvestedAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDetailDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting harvested contracts for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<FarmContractDto>> GetContractsConfirmedPickeUpsAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsConfirmedPickeUpsAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting confirmed pickups for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<FarmContractDto>> GetContractsPaidAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetContractsPaidAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await ToFarmContractDto(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paid contracts for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        #endregion

        #region SALES KPIs
        public async Task<IEnumerable<FarmContractSaleDto>> GetFarmSalesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetFarmSalesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return ToFarmContractSaleDto(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting farm sales for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesAsync(Guid farmId, int year)
        {
            if (farmId == Guid.Empty)
                return default;

            if (year < 2000)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetMonthlySalesAsync(farmId, year).ConfigureAwait(false);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly sales for farm {FarmId} in year {Year}: {Message}", farmId, year, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesByCustomerAsync(Guid farmId, int year)
        {
            if (farmId == Guid.Empty)
                return default;

            if (year < 2000)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetMonthlySalesByCustomerAsync(farmId, year).ConfigureAwait(false);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly sales by customer for farm {FarmId} in year {Year}: {Message}", farmId, year, ex.Message);
                return default;
            }
        }

        public async Task<KeyValuePair<decimal, string>> GetFarmGeneralExpensesCurrentMonthAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var total = await _unitOfWork.Reports.GetFarmGeneralExpensesCurrentMonthAsync(farmId).ConfigureAwait(false);
                return new KeyValuePair<decimal, string>(total, total.ToNumberCompact());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current month general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<KeyValuePair<decimal, string>> GetFarmGeneralExpensesCurrentYearAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var total = await _unitOfWork.Reports.GetFarmGeneralExpensesCurrentYearAsync(farmId).ConfigureAwait(false);
                return new KeyValuePair<decimal, string>(total, total.ToNumberCompact());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current year general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<KeyValuePair<decimal, string>> GetFarmGeneralExpensesPreviousMonthAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var total = await _unitOfWork.Reports.GetFarmGeneralExpensesPreviousMonthAsync(farmId).ConfigureAwait(false);
                return new KeyValuePair<decimal, string>(total, total.ToNumberCompact());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting previous month general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<KeyValuePair<decimal, string>> GetFarmGeneralExpensesPreviousYearAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var total = await _unitOfWork.Reports.GetFarmGeneralExpensesPreviousYearAsync(farmId).ConfigureAwait(false);
                return new KeyValuePair<decimal, string>(total, total.ToNumberCompact());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting previous year general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<KeyValuePair<decimal, string>> GetFarmGeneralExpensesPreviousQuarterAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().Date);
                int currentQuarter = (date.Month - 1) / 3 + 1;

                int prevQuarter = currentQuarter == 1 ? 4 : currentQuarter - 1;
                int year = currentQuarter == 1 ? date.Year - 1 : date.Year;

                var quarterStart = new DateTime(year, (prevQuarter - 1) * 3 + 1, 1);
                var quarterEnd = quarterStart.AddMonths(3).AddDays(-1);

                var ds = DateOnly.FromDateTime(quarterStart);
                var de = DateOnly.FromDateTime(quarterEnd);

                var total = await _unitOfWork.Reports.GetFarmGeneralExpensesPreviousQuarterAsync(farmId, ds, de).ConfigureAwait(false);
                return new KeyValuePair<decimal, string>(total, total.ToNumberCompact());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting previous quarter general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<KeyValuePair<decimal, string>> GetFarmGeneralExpensesCurrentQuarterAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().Date);
                int currentQuarter = (date.Month - 1) / 3 + 1;

                var quarterStart = new DateTime(date.Year, (currentQuarter - 1) * 3 + 1, 1);
                var quarterEnd = quarterStart.AddMonths(3).AddDays(-1);

                var ds = DateOnly.FromDateTime(quarterStart);
                var de = DateOnly.FromDateTime(quarterEnd);

                var total = await _unitOfWork.Reports.GetFarmGeneralExpensesCurrentQuarterAsync(farmId, ds, de).ConfigureAwait(false);
                return new KeyValuePair<decimal, string>(total, total.ToNumberCompact());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current quarter general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                return await _unitOfWork.Reports.Get12MonthExpensesSummaryListAsync(farmId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting 12 month expenses summary for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListPreviousAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                return await _unitOfWork.Reports.Get12MonthExpensesSummaryListPreviousAsync(farmId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting previous year general expenses for farm {FarmId}: {Message}", farmId, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetGrossProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                //return await _unitOfWork.Reports.GetGrossProfitListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await _unitOfWork.FarmKpiSummaries.GetGrossProfitListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gross profit for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                //return await _unitOfWork.Reports.GetNetProfitListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await _unitOfWork.FarmKpiSummaries.GetNetProfitListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitMarginListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                //return await _unitOfWork.Reports.GetNetProfitMarginListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return await _unitOfWork.FarmKpiSummaries.GetNetProfitMarginListAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting net profit margin for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }
        #endregion

        #region CROP PERFORMANCE KPIs
        public async Task<IEnumerable<ProductPerformanceDto>> GetProductPerformancesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetProductPerformancesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product performances for farm {FarmId} from {DateStart} to {DateEnd}: {Message}", farmId, dateStart, dateEnd, ex.Message);
                return default;
            }
        }

        public async Task<IEnumerable<ProductPerformanceDto>> GetProductPerformancesByYearAsync(Guid farmId, int year)
        {
            if (farmId == Guid.Empty)
                return default;

            try
            {
                var list = await _unitOfWork.Reports.GetProductPerformancesByYearAsync(farmId, year).ConfigureAwait(false);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product performances for farm {FarmId} in year {Year}: {Message}", farmId, year, ex.Message);
                return default;
            }
        }
        #endregion
    }
}
