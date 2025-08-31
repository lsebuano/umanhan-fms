using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class FarmActivityUsageRepository : UmanhanRepository<FarmActivityUsage>, IFarmActivityUsageRepository
    {
        public FarmActivityUsageRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmActivityUsage>> GetFarmActivityUsagesAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmActivityUsage> query = _context.FarmActivityUsages.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.Activity.FarmId == farmId)
                        .ToListAsync();
        }

        public Task<List<FarmActivityUsage>> GetFarmActivityUsagesByActivityAsync(Guid activityId, string[] includeEntities)
        {
            IQueryable<FarmActivityUsage> query = _context.FarmActivityUsages.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ActivityId == activityId)
                        .ToListAsync();
        }

        public Task<List<FarmActivityUsage>> GetFarmActivityUsagesByItemAsync(Guid farmId, Guid itemId, string[] includeEntities)
        {
            IQueryable<FarmActivityUsage> query = _context.FarmActivityUsages.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.Activity.FarmId == farmId &&
                                    x.InventoryId == itemId)
                        .ToListAsync();
        }
    }
}
