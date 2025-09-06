using Microsoft.EntityFrameworkCore;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class PricingProfileRepository : UmanhanRepository<PricingProfile>, IPricingProfileRepository
    {
        public PricingProfileRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<PricingProfile>> GetPricingProfilesByFarmIdAsync(Guid farmId, params string[] includeEntities)
        {
            var query = _context.PricingProfiles.AsQueryable().AsSplitQuery();
            if (includeEntities != null && includeEntities.Length > 0)
            {
                foreach (var entity in includeEntities)
                {
                    query = query.Include(entity);
                }
            }
            return query.Where(x => x.FarmId == farmId).ToListAsync();
        }
    }
}
