using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmContractSaleRepository : IRepository<FarmContractSale>
    {
        // add new methods specific to this repository
        Task<List<FarmContractSale>> GetFarmContractSalesAsync(Guid farmId, string[] includeEntities);
    }
}
