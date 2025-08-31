using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmContractDetailRepository : IRepository<FarmContractDetail>
    {
        // add new methods specific to this repository
        Task<List<FarmContractDetail>> GetFarmContractDetailsAsync(Guid contractId, string[] includeEntities);
        Task<List<FarmContractDetail>> GetFarmContractDetailsAsync(Guid farmId, DateTime dateStart, DateTime dateEnd);
    }
}
