using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmActivityPhotoRepository : IRepository<FarmActivityPhoto>
    {
        // add new methods specific to this repository
        Task<List<FarmActivityPhoto>> GetFarmActivityPhotosByActivityAsync(Guid activityId, params string[] includeEntities);
    }
}
