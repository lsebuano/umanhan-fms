using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class FarmTransactionRepository : UmanhanRepository<FarmTransaction>, IFarmTransactionRepository
    {
        public FarmTransactionRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public async Task<List<FarmTransaction>> GetFarmTransactionsAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmTransaction> query = _context.FarmTransactions.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return await query.Where(x => x.FarmId == farmId)
                        .ToListAsync()
                        .ConfigureAwait(false);
        }

        public Task<List<FarmTransaction>> GetRecentFarmTransactionsAsync(Guid farmId)
        {
            // Get the last 10 sales transactions for the specified farm for the current week
            DateTime today = DateTime.Today.ToLocalTime();
            int diff = (int)today.DayOfWeek;
            DateOnly weekStart = DateOnly.FromDateTime(today.AddDays(-1 * diff));
            DateOnly weekEnd = weekStart.AddDays(6);

            return _context.FarmTransactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.TransactionType)
                .Include(x => x.Unit)
                .Where(x => x.FarmId == farmId &&
                            (x.Date >= weekStart && x.Date <= weekEnd))
                .OrderByDescending(x => x.Date)
                .Take(5)
                .ToListAsync();
        }
    }
}
