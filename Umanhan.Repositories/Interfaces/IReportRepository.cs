using Umanhan.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IReportRepository
    {
        // add new methods specific to this repository
        Task<IEnumerable<dynamic>> ExecuteReportQueryAsync(string sql, CancellationToken ct = default);

        Task<decimal> GetCostOfGoodsSoldAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetGrossMarginPercentAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetNetProfitMarginAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetOperatingExpensesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetOperatingExpenseRatioAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetTotalRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetYieldPerHectareAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);

        Task<decimal> GetGrossProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetNetProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);

        Task<Dictionary<string, decimal>> GetTotalDonatedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<Dictionary<string, decimal>> GetTotalSpoilageAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);

        //Task<IEnumerable<SparklineChartDto>> GetGrossMarginPercentListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        //Task<IEnumerable<SparklineChartDto>> GetYieldPerHectareListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetTotalRevenueListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetTotalDonatedListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetTotalSpoilageListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListAsync(Guid farmId);
        Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListPreviousAsync(Guid farmId);
        Task<IEnumerable<SparklineChartDto>> GetGrossProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetNetProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetNetProfitMarginListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);

        Task<IEnumerable<FarmContractDetail>> GetContractsApproachingHarvestAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContractDetail>> GetContractsHarvestedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContract>> GetContractsNewAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContract>> GetContractsPaidAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContract>> GetContractsConfirmedPickeUpsAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContractDetail>> GetContractsTotalValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContractDetail>> GetContractsTotalLostValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContractDetail>> GetContractsExpectedRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<FarmContractSale>> GetFarmSalesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesAsync(Guid farmId, int year);
        Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesByCustomerAsync(Guid farmId, int year);

        Task<decimal> GetFarmGeneralExpensesCurrentMonthAsync(Guid farmId);
        Task<decimal> GetFarmGeneralExpensesCurrentYearAsync(Guid farmId);
        Task<decimal> GetFarmGeneralExpensesPreviousMonthAsync(Guid farmId);
        Task<decimal> GetFarmGeneralExpensesPreviousYearAsync(Guid farmId);
        Task<decimal> GetFarmGeneralExpensesCurrentQuarterAsync(Guid farmId, DateOnly startDate, DateOnly endDate);
        Task<decimal> GetFarmGeneralExpensesPreviousQuarterAsync(Guid farmId, DateOnly startDate, DateOnly endDate);

        Task<IEnumerable<ProductPerformanceDto>> GetProductPerformancesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<ProductPerformanceDto>> GetProductPerformancesByYearAsync(Guid farmId, int year);
    }
}
