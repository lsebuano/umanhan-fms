using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmTransactionRepository : IRepository<FarmTransaction>
    {
        // add new methods specific to this repository
        Task<List<FarmTransaction>> GetFarmTransactionsAsync(Guid farmId, string[] includeEntities);
        Task<List<FarmTransaction>> GetRecentFarmTransactionsAsync(Guid farmId);
    }
}
