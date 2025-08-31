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
    public class FarmCropRepository : UmanhanRepository<FarmCrop>, IFarmCropRepository
    {
        public FarmCropRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmCrop>> GetCropsByFarmAsync(Guid farmId, params string[] includeEntities)
        {
            IQueryable<FarmCrop> query = _context.FarmCrops.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId)
                        .ToListAsync();
        }

        public Task<FarmCrop> GetByCropAsync(Guid cropId, params string[] includeEntities)
        {
            IQueryable<FarmCrop> query = _context.FarmCrops.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.FirstOrDefaultAsync(x => x.CropId == cropId);
        }
    }
}
