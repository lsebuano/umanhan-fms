using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmInventoryRepository : IRepository<FarmInventory>
    {
        // add new methods specific to this repository
        Task<List<FarmInventory>> GetFarmInventoriesAsync(Guid farmId, params string[] includeEntities);
        Task<FarmInventory> GetFarmInventoryByIdAsync(Guid farmId, Guid itemId, params string[] includeEntities);
        void UpdateQuantity(Guid inventoryId, decimal newValue);
    }
}
