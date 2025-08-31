using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Repositories.Interfaces
{
    public interface IPricingConditionRepository : IRepository<PricingCondition>
    {
        // add new methods specific to this repository
        Task<List<PricingCondition>> GetPricingsByFarmIdAsync(Guid farmId, params string[] includeEntities);
        Task<List<PricingCondition>> GetPricingsByProfileIdAsync(Guid profileId, params string[] includeEntities);
        Task UpdateBatchAsync(IEnumerable<PricingCondition> ordered);
    }
}
