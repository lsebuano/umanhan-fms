using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;
using Umanhan.Dtos;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Shared;
using TransactionType = Umanhan.Dtos.HelperModels.TransactionType;

namespace Umanhan.Repositories
{
    public class ReportRepository : UmanhanRepository<Farm>, IReportRepository
    {
        private bool IsSelectOnly(string sql)
        {
            var trimmed = sql.TrimStart().ToUpperInvariant();
            return (trimmed.StartsWith("SELECT") || trimmed.StartsWith("EXPLAIN") || trimmed.StartsWith("WITH")) && !Regex.IsMatch(trimmed, @"\b(INSERT|UPDATE|DELETE|DROP|ALTER|CREATE|GRANT|TRUNCATE)\b", RegexOptions.IgnoreCase);
        }

        public ReportRepository(UmanhanDbContext context) : base(context)
        {

        }

        // add new methods specific to this repository
        public async Task<IEnumerable<dynamic>> ExecuteReportQueryAsync(string sql, CancellationToken ct = default)
        {
            if (!IsSelectOnly(sql))
                throw new InvalidOperationException("Only SELECT statements are allowed.");

            var results = new List<Dictionary<string, object>>();

            var conn = _context.Database.GetDbConnection();
            await using var command = conn.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(ct).ConfigureAwait(false);

            await using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = await reader.IsDBNullAsync(i, ct).ConfigureAwait(false) ? null : reader.GetValue(i);
                    row[reader.GetName(i)] = value;
                }
                results.Add(row);
            }

            return results;
        }

        #region DASHBOARD KPIs
        public async Task<decimal> GetCostOfGoodsSoldAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var totalExpenses = await _context.FarmActivityExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                // get relevant expenses (the overlaps)
                .Where(u => u.Activity.FarmId == farmId &&
                            u.Activity.StartDate <= dateEnd &&
                            u.Activity.EndDate >= dateStart)
                .SumAsync(e => e.Amount);

            var totalLabor = await _context.FarmActivityLaborers
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                .Where(u => u.Activity.FarmId == farmId &&
                            u.Activity.StartDate <= dateEnd &&
                            u.Activity.EndDate >= dateStart)
                .SumAsync(l => l.TotalPayment);

            var totalUsage = await _context.FarmActivityUsages
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                .Where(u => u.Activity.FarmId == farmId &&
                            u.Activity.StartDate <= dateEnd &&
                            u.Activity.EndDate >= dateStart)
                .SumAsync(u => u.TotalCost);

            var cogs = totalExpenses
                       + totalLabor
                       + totalUsage;

            return cogs;
        }

        public async Task<decimal> GetGrossMarginPercentAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalRevenue = await GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            if (totalRevenue == 0)
                return 0m; // Avoid division by zero

            var cogs = await GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);

            var grossMarginPct = totalRevenue > 0 ? (totalRevenue - cogs) / totalRevenue : 0m;
            return grossMarginPct;
        }

        public async Task<decimal> GetNetProfitMarginAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var totalRevenue = await GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            if (totalRevenue == 0)
                return 0m; // Avoid division by zero

            var cogs = await GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            var operatingExpenses = await GetOperatingExpensesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);

            var netProfit = totalRevenue - cogs - operatingExpenses;
            var netProfitMargin = totalRevenue > 0
                ? netProfit / totalRevenue
                : 0m;

            return netProfitMargin;
        }

        public async Task<decimal> GetOperatingExpensesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var operatingExpenses = await _context.FarmGeneralExpenses
                .AsNoTracking()
                .Where(u => u.FarmId == farmId &&
                            u.Date >= ds &&
                            u.Date <= de)
                .SumAsync(e => e.Amount);

            return operatingExpenses;
        }

        public async Task<decimal> GetOperatingExpenseRatioAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var totalRevenue = await GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            if (totalRevenue == 0)
                return 0m; // Avoid division by zero

            var operatingExpenses = await GetOperatingExpensesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);

            var operatingExpenseRatio = totalRevenue > 0
                ? operatingExpenses / totalRevenue
                : 0m;

            return operatingExpenseRatio;
        }

        public async Task<decimal> GetTotalRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalTransactions = await _context.FarmTransactions
                .AsNoTracking()
                .Where(x => x.FarmId == farmId &&
                            x.Date >= ds &&
                            x.Date <= de)
                .SumAsync(t => t.TotalAmount)
                .ConfigureAwait(false);

            var totalSales = await _context.FarmContractSales
                .AsNoTracking()
                .Where(c => c.FarmId == farmId &&
                            c.Date >= ds &&
                            c.Date <= de)
                .SumAsync(s => s.TotalAmount)
                .ConfigureAwait(false);

            return totalTransactions + totalSales;
        }

        public async Task<decimal> GetYieldPerHectareAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalYield = await _context.FarmZoneYields
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Zone)
                .Where(y => y.Zone.FarmId == farmId)
                .SumAsync(y => y.ActualYield ?? 0m);

            var totalHectares = await _context.Farms
                .AsNoTracking()
                .Where(f => f.Id == farmId)
                .SumAsync(f => f.SizeInHectares);

            if (totalHectares == 0)
                return 0m; // Avoid division by zero

            var yieldPerHectare = totalHectares > 0
                ? totalYield / totalHectares
                : 0m;

            return (decimal)yieldPerHectare;
        }


        public async Task<decimal> GetGrossProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalRevenue = await GetTotalRevenueAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            var totalCogs = await GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);

            return totalRevenue - totalCogs;
        }

        public async Task<decimal> GetNetProfitAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var grossProfit = await GetGrossProfitAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);
            var totalOpex = await GetOperatingExpensesAsync(farmId, dateStart, dateEnd).ConfigureAwait(false);

            return grossProfit - totalOpex;
        }

        public async Task<Dictionary<string, decimal>> GetTotalDonatedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalTransactions = await _context.FarmTransactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.TransactionType)
                .Include(x => x.Unit)
                .Where(x => x.FarmId == farmId &&
                            x.Date >= ds &&
                            x.Date <= de &&
                            x.TransactionType.TransactionTypeName.ToUpper() == TransactionType.DONATION.ToString())
                .GroupBy(x => x.Unit.UnitName)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Sum(t => t.Quantity))
                .ConfigureAwait(false);

            return totalTransactions;
        }

        public async Task<Dictionary<string, decimal>> GetTotalSpoilageAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalTransactions = await _context.FarmTransactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.TransactionType)
                .Include(x => x.Unit)
                .Where(x => x.FarmId == farmId &&
                            x.Date >= ds &&
                            x.Date <= de &&
                            x.TransactionType.TransactionTypeName.ToUpper() == TransactionType.SPOILAGE.ToString())
                .GroupBy(x => x.Unit.UnitName)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Sum(t => t.Quantity))
                .ConfigureAwait(false);

            return totalTransactions;
        }


        public async Task<IEnumerable<SparklineChartDto>> GetCostOfGoodsSoldListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var totalExpenses = await _context.FarmActivityExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                // get relevant expenses (the overlaps)
                .Where(u => u.Activity.FarmId == farmId &&
                            (u.Activity.StartDate <= dateEnd) &&
                            u.Activity.EndDate >= dateStart)
                .Select(x => new
                {
                    Date = x.Date.ToDateTime(TimeOnly.MinValue),
                    Amount = x.Amount
                })
                .ToListAsync();

            var totalLabor = await _context.FarmActivityLaborers
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                .Where(u => u.Activity.FarmId == farmId &&
                            u.Activity.StartDate <= dateEnd &&
                            u.Activity.EndDate >= dateStart)
                .Select(x => new
                {
                    Date = x.Timestamp,
                    Amount = x.TotalPayment
                })
                .ToListAsync();

            var totalUsage = await _context.FarmActivityUsages
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                .Where(u => u.Activity.FarmId == farmId &&
                            u.Activity.StartDate <= dateEnd &&
                            u.Activity.EndDate >= dateStart)
                .Select(x => new
                {
                    Date = x.Timestamp,
                    Amount = x.TotalCost
                })
                .ToListAsync();

            var grouped = totalExpenses.Concat(totalLabor).Concat(totalUsage)
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Date = DateOnly.FromDateTime(month),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var operatingExpenses = await _context.FarmGeneralExpenses
                .AsNoTracking()
                .Where(u => u.FarmId == farmId &&
                            u.Date >= ds &&
                            u.Date <= de)
                .Select(x => new
                {
                    x.Date,
                    x.Amount
                })
                .ToListAsync();

            var grouped = operatingExpenses
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            // Merge with full month range
            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Date = DateOnly.FromDateTime(month),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetOperatingExpenseRatioListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var operatingExpenses = await _context.FarmActivityExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Activity)
                .Where(u => u.Activity.FarmId == farmId &&
                            u.Activity.StartDate <= dateEnd &&
                            u.Activity.EndDate >= dateStart)
                .Select(x => new
                {
                    x.Date,
                    x.Amount
                })
                .ToListAsync();

            var grouped = operatingExpenses
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            // Merge with full month range
            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Date = DateOnly.FromDateTime(month),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalRevenueListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalTransactions = await _context.FarmTransactions
                .AsNoTracking()
                .Where(x => x.FarmId == farmId &&
                            x.Date >= ds &&
                            x.Date <= de)
                .Select(x => new
                {
                    x.Date,
                    Amount = x.TotalAmount
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var totalSales = await _context.FarmContractSales
                .AsNoTracking()
                .Where(c => c.FarmId == farmId &&
                            c.Date >= ds &&
                            c.Date <= de)
                .Select(x => new
                {
                    x.Date,
                    Amount = x.TotalAmount
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var grouped = totalTransactions.Concat(totalSales)
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            // Merge with full month range
            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Date = DateOnly.FromDateTime(month),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }


        public async Task<IEnumerable<SparklineChartDto>> GetTotalDonatedListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalTransactions = await _context.FarmTransactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.TransactionType)
                .Where(x => x.FarmId == farmId &&
                            x.Date >= ds &&
                            x.Date <= de &&
                            x.TransactionType.TransactionTypeName.ToUpper() == TransactionType.DONATION.ToString())
                .Select(x => new
                {
                    x.Date,
                    Amount = x.Quantity
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var grouped = totalTransactions
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            // Merge with full month range
            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Date = DateOnly.FromDateTime(month),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetTotalSpoilageListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            var totalTransactions = await _context.FarmTransactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.TransactionType)
                .Where(x => x.FarmId == farmId &&
                            x.Date >= ds &&
                            x.Date <= de &&
                            x.TransactionType.TransactionTypeName.ToUpper() == TransactionType.SPOILAGE.ToString())
                .Select(x => new
                {
                    x.Date,
                    Amount = x.Quantity
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var grouped = totalTransactions
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            // Merge with full month range
            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Date = DateOnly.FromDateTime(month),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> GetGrossProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SparklineChartDto>> GetNetProfitMarginListAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CONTRACTS KPIs
        public async Task<IEnumerable<FarmContractDetail>> GetContractsApproachingHarvestAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);
            var today = DateOnly.FromDateTime(DateTime.Today);
            var in7Days = DateOnly.FromDateTime(DateTime.Now.AddDays(7));

            var query = _context.FarmContractDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Contract)
                .Where(c => c.Contract.FarmId == farmId &&
                            c.Contract.ContractDate >= ds &&
                            c.Contract.ContractDate <= de);
            //approaching harvest logic, starting within 7 days
            return await query.Where(c => c.HarvestDate >= today &&
                                          c.HarvestDate <= in7Days &&
                                          c.Status != ContractStatus.CANCELLED.ToString() &&
                                          c.Status != ContractStatus.PICKUP_CONFIRMED.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContractDetail>> GetContractsHarvestedAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContractDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Contract)
                .Where(c => c.Contract.FarmId == farmId &&
                            c.HarvestDate >= ds &&
                            c.HarvestDate <= de &&
                            c.Status.ToUpper() == ContractStatus.PICKUP_CONFIRMED.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContract>> GetContractsNewAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContracts
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.FarmContractDetails)
                .Where(c => c.FarmId == farmId &&
                            c.ContractDate >= ds &&
                            c.ContractDate <= de &&
                            c.Status.ToUpper() == ContractStatus.NEW.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContract>> GetContractsPaidAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContracts
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.FarmContractDetails)
                .Where(c => c.FarmId == farmId &&
                            c.ContractDate >= ds &&
                            c.ContractDate <= de &&
                            c.Status.ToUpper() == ContractStatus.PAID.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContract>> GetContractsConfirmedPickeUpsAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContracts
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.FarmContractDetails)
                .Where(c => c.FarmId == farmId &&
                            c.ContractDate >= ds &&
                            c.ContractDate <= de &&
                            c.Status.ToLower() == ContractStatus.PICKUP_CONFIRMED.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContractDetail>> GetContractsTotalValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContractDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Contract)
                .Where(c => c.Contract.FarmId == farmId &&
                            c.Contract.ContractDate >= ds &&
                            c.Contract.ContractDate <= de &&
                            (c.Status.ToUpper() == ContractStatus.NEW.ToString() ||
                             c.Status.ToUpper() == ContractStatus.PICKUP_CONFIRMED.ToString() ||
                             c.Status.ToUpper() == ContractStatus.PARTIALLY_PAID.ToString()))
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContractDetail>> GetContractsTotalLostValueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContractDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Contract)
                .Where(c => c.Contract.FarmId == farmId &&
                            c.Contract.ContractDate >= ds &&
                            c.Contract.ContractDate <= de &&
                            c.Status.ToUpper() == ContractStatus.CANCELLED.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FarmContractDetail>> GetContractsExpectedRevenueAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContractDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Contract)
                .Where(c => c.Contract.FarmId == farmId &&
                            c.Contract.ContractDate >= ds &&
                            c.Contract.ContractDate <= de &&
                            c.Status.ToUpper() != ContractStatus.CANCELLED.ToString())
                .ToListAsync()
                .ConfigureAwait(false);
        }
        #endregion

        #region SALES KPIs
        public async Task<IEnumerable<FarmContractSale>> GetFarmSalesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            var ds = DateOnly.FromDateTime(dateStart.Date);
            var de = DateOnly.FromDateTime(dateEnd.Date).AddDays(1);

            return await _context.FarmContractSales
                .AsNoTracking()
                .Where(c => c.FarmId == farmId &&
                            c.Date >= ds &&
                            c.Date <= de)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesAsync(Guid farmId, int year)
        {
            return await _context.FarmContractSales
                .AsNoTracking()
                .Where(c => c.FarmId == farmId &&
                            c.Date.Year == year)
                .GroupBy(c => new { c.Date.Year, c.Date.Month })
                .Select(g => new MonthlySalesDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesByCustomerAsync(Guid farmId, int year)
        {
            return await _context.FarmContractSales
                .AsNoTracking()
                .Where(c => c.FarmId == farmId &&
                            c.Date.Year == year)
                .GroupBy(c => new { c.Date.Year, c.Date.Month, c.CustomerName })
                .Select(g => new MonthlySalesDto
                {
                    Customer = g.Key.CustomerName,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }
        #endregion

        #region FARM GENERAL EXPENSES
        public Task<decimal> GetFarmGeneralExpensesCurrentMonthAsync(Guid farmId)
        {
            var date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().Date);

            var total = _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.FarmId == farmId &&
                            x.Date.Month == date.Month &&
                            x.Date.Year == date.Year)
                .SumAsync(x => x.Amount);

            return total;
        }

        public Task<decimal> GetFarmGeneralExpensesCurrentYearAsync(Guid farmId)
        {
            var date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().Date);

            var total = _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.FarmId == farmId &&
                            x.Date.Year == date.Year)
                .SumAsync(x => x.Amount);

            return total;
        }

        public Task<decimal> GetFarmGeneralExpensesPreviousMonthAsync(Guid farmId)
        {
            var date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().Date);

            var total = _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.FarmId == farmId &&
                            x.Date.Month == date.Month - 1 &&
                            x.Date.Year == date.Year)
                .SumAsync(x => x.Amount);

            return total;
        }

        public Task<decimal> GetFarmGeneralExpensesPreviousYearAsync(Guid farmId)
        {
            var date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime().Date);

            var total = _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.FarmId == farmId &&
                            x.Date.Year == date.Year - 1)
                .SumAsync(x => x.Amount);

            return total;
        }

        public Task<decimal> GetFarmGeneralExpensesPreviousQuarterAsync(Guid farmId, DateOnly startDate, DateOnly endDate)
        {
            var total = _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.FarmId == farmId &&
                            x.Date >= startDate &&
                            x.Date <= endDate)
                .SumAsync(x => x.Amount);

            return total;
        }

        public Task<decimal> GetFarmGeneralExpensesCurrentQuarterAsync(Guid farmId, DateOnly startDate, DateOnly endDate)
        {
            var total = _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.FarmId == farmId &&
                            x.Date >= startDate &&
                            x.Date <= endDate)
                .SumAsync(x => x.Amount);

            return total;
        }

        public async Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListAsync(Guid farmId)
        {
            var date = DateTime.Now.ToLocalTime();
            var totalExpenses = await _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(u => u.FarmId == farmId &&
                            u.Date.Year == date.Year)
                .Select(x => new
                {
                    Date = x.Date.ToDateTime(TimeOnly.MinValue),
                    Amount = x.Amount
                })
                .ToListAsync();

            var grouped = totalExpenses
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var dateStart = new DateTime(date.Year, 1, 1);
            var dateEnd = new DateTime(date.Year, 12, 1);

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Month = month.Month,
                    MonthString = month.ToString("MMM"),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SparklineChartDto>> Get12MonthExpensesSummaryListPreviousAsync(Guid farmId)
        {
            var date = DateTime.Now.ToLocalTime();
            var totalExpenses = await _context.FarmGeneralExpenses
                .AsNoTracking()
                .AsSplitQuery()
                .Where(u => u.FarmId == farmId &&
                            u.Date.Year == date.Year - 1)
                .Select(x => new
                {
                    Date = x.Date.ToDateTime(TimeOnly.MinValue),
                    Amount = x.Amount
                })
                .ToListAsync();

            var grouped = totalExpenses
                .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            var dateStart = new DateTime(date.Year - 1, 1, 1);
            var dateEnd = new DateTime(date.Year - 1, 12, 1);

            var allMonths = new List<DateTime>();
            for (var d = dateStart; d <= dateEnd; d = d.AddMonths(1))
            {
                allMonths.Add(d);
            }

            var result = allMonths
                .Select(month => new SparklineChartDto
                {
                    Month = month.Month,
                    MonthString = month.ToString("MMM"),
                    Total = grouped.TryGetValue(month, out var total) ? total : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }
        #endregion

        #region PRODUCT PERFORMANCE KPIs
        public async Task<IEnumerable<ProductPerformanceDto>> GetProductPerformancesAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            // use dapper!
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var result = await conn.QueryAsync<ProductPerformanceDto>(
                "SELECT * FROM fn_get_product_performance(@in_farm_id,@in_date_start,@in_date_end)",
                new
                {
                    in_farm_id = farmId,
                    in_date_start = dateStart.Date,
                    in_date_end = dateEnd.Date
                }
            );
            return [.. result];
            // Do NOT dispose the connection
            // Let EF Core dispose it with the DbContext
        }

        public async Task<IEnumerable<ProductPerformanceDto>> GetProductPerformancesByYearAsync(Guid farmId, int year)
        {
            // use dapper!
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var result = await conn.QueryAsync<ProductPerformanceDto>(
                "SELECT * FROM fn_get_product_performance_by_year(@in_farm_id, @in_year)",
                new
                {
                    in_farm_id = farmId,
                    in_year = year
                }
            );
            return [.. result];
            // Do NOT dispose the connection
            // Let EF Core dispose it with the DbContext
        }


        #endregion
    }
}
