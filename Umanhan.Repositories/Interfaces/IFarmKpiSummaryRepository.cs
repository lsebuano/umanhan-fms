using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmKpiSummaryRepository : IRepository<FarmKpiSummary>
    {
        // add new methods specific to this repository
        Task<decimal> GetCostOfGoodsSoldAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetGrossMarginPercentAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetNetProfitMarginAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetOperatingExpensesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetOperatingExpenseRatioAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetTotalRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetGrossProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<decimal> GetNetProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);

        Task<IEnumerable<SparklineChartDto>> GetGrossMarginPercentListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetYieldPerHectareListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetTotalRevenueListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetTotalDonatedListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetTotalSpoilageListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetGrossProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetNetProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
        Task<IEnumerable<SparklineChartDto>> GetNetProfitMarginListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
    }
}
