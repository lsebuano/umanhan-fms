using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IPricingProfileRepository : IRepository<PricingProfile>
    {
        // add new methods specific to this repository
        Task<List<PricingProfile>> GetPricingProfilesByFarmIdAsync(Guid farmId, params string[] includeEntities);
    }
}
