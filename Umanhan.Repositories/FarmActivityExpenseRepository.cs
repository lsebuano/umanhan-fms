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
    public class FarmActivityExpenseRepository : UmanhanRepository<FarmActivityExpense>, IFarmActivityExpenseRepository
    {
        public FarmActivityExpenseRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmActivityExpense>> GetFarmActivityExpensesAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmActivityExpense> query = _context.FarmActivityExpenses.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.Activity.FarmId == farmId)
                        .ToListAsync();
        }

        public Task<List<FarmActivityExpense>> GetFarmActivityExpensesByActivityAsync(Guid activityId, string[] includeEntities)
        {
            IQueryable<FarmActivityExpense> query = _context.FarmActivityExpenses.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ActivityId == activityId)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync();
        }
    }
}
