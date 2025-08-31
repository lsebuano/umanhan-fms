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
    public class FarmLivestockRepository : UmanhanRepository<FarmLivestock>, IFarmLivestockRepository
    {
        public FarmLivestockRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmLivestock>> GetLivestocksByFarmAsync(Guid farmId, params string[] includeEntities)
        {
            IQueryable<FarmLivestock> query = _context.FarmLivestocks.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId)
                        .ToListAsync();
        }
    }
}
