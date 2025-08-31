using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmContractRepository : IRepository<FarmContract>
    {
        // add new methods specific to this repository
        Task<List<FarmContract>> GetFarmContractsAsync(Guid farmId, string[] includeEntities);
        Task<List<FarmContract>> GetFarmContractsAsync(Guid farmId, DateTime startDate, DateTime endDate, string[] includeEntities);
        //Dictionary<ProductKey, ProductDto> BuildProductLookup();
    }
}
