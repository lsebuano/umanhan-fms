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
    public class FarmActivityRepository : UmanhanRepository<FarmActivity>, IFarmActivityRepository
    {
        public FarmActivityRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmActivity>> GetFarmActivitiesAsync(Guid farmId)
        {
            return _context.FarmActivities.AsNoTracking()
                                          .AsSplitQuery()
                                          .Include(x => x.Farm)
                                          .Include(x => x.Task)
                                          .Where(x => x.FarmId == farmId)
                                          .ToListAsync();
        }

        public Task<List<FarmActivity>> GetFarmActivitiesAsync(Guid farmId, DateTime date, params string[] includeEntities)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            IQueryable<FarmActivity> query = _context.FarmActivities.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId &&
                                    (x.StartDate <= date &&
                                    (x.EndDate == null || x.EndDate >= date)) || // include rows where date is within the range & also the records with no enddates
                                    date < x.StartDate // get also the future activities
                                )
                        .OrderBy(x => x.StartDate)
                        .ToListAsync();
        }

        public Task<List<FarmActivityExpense>> GetFarmActivityExpensesAsync(Guid activityId, params string[] includeEntities)
        {
            IQueryable<FarmActivityExpense> query = _context.FarmActivityExpenses.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ActivityId == activityId)
                        .ToListAsync();
        }
    }
}
