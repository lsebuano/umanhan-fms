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
    public class FarmActivityLaborerRepository : UmanhanRepository<FarmActivityLaborer>, IFarmActivityLaborerRepository
    {
        public FarmActivityLaborerRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmActivityLaborer>> GetFarmActivityLaborersAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmActivityLaborer> query = _context.FarmActivityLaborers.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.Activity.FarmId == farmId)
                        .ToListAsync();
        }

        public Task<List<FarmActivityLaborer>> GetFarmActivityLaborersByActivityAsync(Guid activityId, string[] includeEntities)
        {
            IQueryable<FarmActivityLaborer> query = _context.FarmActivityLaborers.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ActivityId == activityId)
                        .ToListAsync();
        }
    }
}
