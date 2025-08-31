using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<VwProductLookup>
    {

        // add new methods specific to this repository
        Task<List<VwProductLookup>> GetProductsByFarmAsync(Guid farmId);
        Task<List<VwProductLookup>> GetProductsByFarmByTypeAsync(Guid farmId, Guid typeId);
        Task<VwProductLookup> GetProductByIdAsync(Guid typeId, Guid id);
        Task<Dictionary<ProductKey, ProductDto>> BuildProductLookupAsync();
    }
}
