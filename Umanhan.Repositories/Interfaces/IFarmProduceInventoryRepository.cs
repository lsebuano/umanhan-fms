using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmProduceInventoryRepository : IRepository<FarmProduceInventory>
    {
        // add new methods specific to this repository
        Task<List<FarmProduceInventory>> GetFarmProduceInventoriesAsync(Guid farmId, params string[] includeEntities);
        Task<List<FarmProduceInventory>> GetFarmProduceInventoriesAsync(Guid farmId, Guid typeId, params string[] includeEntities);
    }
}
