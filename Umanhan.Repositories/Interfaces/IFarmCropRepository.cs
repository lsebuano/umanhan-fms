using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmCropRepository : IRepository<FarmCrop>
    {
        // add new methods specific to this repository
        Task<List<FarmCrop>> GetCropsByFarmAsync(Guid farmId, params string[] includeEntities);
        Task<FarmCrop> GetByCropAsync(Guid cropId, params string[] includeEntities);
    }
}
