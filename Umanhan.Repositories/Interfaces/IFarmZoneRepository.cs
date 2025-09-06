using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmZoneRepository : IRepository<FarmZone>
    {
        // add new methods specific to this repository
        Task<List<FarmZone>> GetFarmZonesByFarmAsync(Guid farmId, string[] includeEntities);
    }
}
