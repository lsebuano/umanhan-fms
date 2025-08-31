using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Shared.Extensions;
using static System.Net.Mime.MediaTypeNames;
using TransactionType = Umanhan.Models.Models.TransactionType;

namespace Umanhan.Repositories
{
    public class FarmKpiSummaryRepository : UmanhanRepository<FarmKpiSummary>, IFarmKpiSummaryRepository
    {
        public FarmKpiSummaryRepository(UmanhanDbContext context) : base(context)
        {

        }

        #region DASHBOARD KPIs
        public async Task<decimal> GetCostOfGoodsSoldAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var cogs = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "cost_of_goods_sold" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return cogs;
        }

        public async Task<decimal> GetGrossMarginPercentAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var grossMarginPct = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "gross_margin_pct" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return grossMarginPct;
        }

        public async Task<decimal> GetNetProfitMarginAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var netProfitMargin = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "net_profit_margin" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return netProfitMargin;
        }

        public async Task<decimal> GetOperatingExpensesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var operatingExpenses = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "operating_expense" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return operatingExpenses;
        }

        public async Task<decimal> GetOperatingExpenseRatioAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var operatingExpenseRatio = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "operating_expense_ratio" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return operatingExpenseRatio;
        }

        public async Task<decimal> GetTotalRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalRevenue = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "total_revenue" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return totalRevenue;
        }

        public async Task<decimal> GetGrossProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var grossProfit = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "gross_profit" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return grossProfit;
        }

        public async Task<decimal> GetNetProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var netProfit = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "net_profit" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            u.PeriodDate >= ds &&
                            u.PeriodDate <= de)
                .SumAsync(u => u.KpiValue);

            return netProfit;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetGrossMarginPercentListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "gross_margin_pct" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public Task<IEnumerable<SparklineChartDto>> GetYieldPerHectareListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SparklineChartDto>> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "cost_of_goods_sold" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "operating_expense" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "operating_expense_ratio" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalRevenueListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "total_revenue" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalDonatedListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "total_donated" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalSpoilageListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "total_spoilage" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetGrossProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "gross_profit" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "net_profit" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitMarginListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date);

            var earliestPrevYear = ds.AddYears(-1);
            var latestPrevYear = de.AddYears(-1);

            var list = await _dbSet.AsNoTracking()
                .Where(u => u.KpiName == "net_profit_margin" &&
                            u.PeriodType == "monthly" &&
                            u.FarmId == farmId &&
                            ((u.PeriodDate >= ds && u.PeriodDate <= de) ||
                            (u.PeriodDate >= earliestPrevYear && u.PeriodDate <= latestPrevYear)))
                .ToDictionaryAsync(g => g.PeriodDate, g => g.KpiValue);

            var allMonths = new List<DateOnly>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(DateOnly.FromDateTime(d));
            }

            var result = allMonths
                .Select(currentMonth =>
                {
                    var currentTotal = list.TryGetValue(currentMonth, out var val) ? val : 0;

                    var previousYearSameMonth = currentMonth.AddYears(-1);
                    var previousTotal = list.TryGetValue(previousYearSameMonth, out var prevVal) ? prevVal : 0;

                    return new SparklineChartDto
                    {
                        Date = currentMonth,
                        Total = currentTotal,
                        TotalPrevious = previousTotal,
                        Month = currentMonth.Month,
                        MonthString = currentMonth.ToString("MMM")
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        #endregion
    }
}
