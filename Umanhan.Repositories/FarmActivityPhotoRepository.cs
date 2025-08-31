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
    public class FarmActivityPhotoRepository : UmanhanRepository<FarmActivityPhoto>, IFarmActivityPhotoRepository
    {
        public FarmActivityPhotoRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository
        public Task<List<FarmActivityPhoto>> GetFarmActivityPhotosByActivityAsync(Guid activityId, params string[] includeEntities)
        {
            IQueryable<FarmActivityPhoto> query = _context.FarmActivityPhotos.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ActivityId == activityId)
                        .ToListAsync();
        }
    }
}
