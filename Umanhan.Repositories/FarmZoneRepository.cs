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
    public class FarmZoneRepository : UmanhanRepository<FarmZone>, IFarmZoneRepository
    {
        public FarmZoneRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmZone>> GetFarmZonesByFarmAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmZone> query = _context.FarmZones.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId)
                        .ToListAsync();
        }
    }
}
