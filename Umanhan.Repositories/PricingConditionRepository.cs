using Microsoft.EntityFrameworkCore;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Repositories
{
    public class PricingConditionRepository : UmanhanRepository<PricingCondition>, IPricingConditionRepository
    {
        public PricingConditionRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<PricingCondition>> GetPricingsByFarmIdAsync(Guid farmId, params string[] includeEntities)
        {
            var query = _context.PricingConditions.AsQueryable().AsSplitQuery();
            if (includeEntities != null && includeEntities.Length > 0)
            {
                foreach (var entity in includeEntities)
                {
                    query = query.Include(entity);
                }
            }
            return query.Where(x => x.PricingProfile.FarmId == farmId)
                .OrderBy(x => x.Sequence)
                .ToListAsync();
        }

        public Task<List<PricingCondition>> GetPricingsByProfileIdAsync(Guid profileId, params string[] includeEntities)
        {
            var query = _context.PricingConditions.AsQueryable().AsSplitQuery();
            if (includeEntities != null && includeEntities.Length > 0)
            {
                foreach (var entity in includeEntities)
                {
                    query = query.Include(entity);
                }
            }
            return query.Where(x => x.ProfileId == profileId)
                .OrderBy(x => x.Sequence)
                .ToListAsync();
        }

        public async Task UpdateBatchAsync(IEnumerable<PricingCondition> ordered)
        {
            foreach (var updated in ordered)
            {
                // This will give you the tracked entity (or attach a new one if not yet tracked)
                var existing = await _context.PricingConditions.FindAsync(updated.Id);
                if (existing != null)
                {
                    existing.Sequence = updated.Sequence;
                }
            }
        }
    }
}
