using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class FarmGeneralExpenseRepository : UmanhanRepository<FarmGeneralExpense>, IFarmGeneralExpenseRepository
    {
        public FarmGeneralExpenseRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmGeneralExpense>> GetFarmGeneralExpensesAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmGeneralExpense> query = _context.FarmGeneralExpenses.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId)
                        .ToListAsync();
        }

        public Task<List<FarmGeneralExpense>> GetFarmGeneralExpensesAsync(Guid farmId, DateTime startDate, DateTime endDate, string[] includeEntities)
        {
            var ds = DateOnly.FromDateTime(startDate.Date);
            var de = DateOnly.FromDateTime(endDate.Date).AddDays(1);

            IQueryable<FarmGeneralExpense> query = _context.FarmGeneralExpenses.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId &&
                                    x.Date >= ds &&
                                    x.Date <= de)
                        .ToListAsync();
        }
    }
}
