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
    public interface IPricingProfileRepository : IRepository<PricingProfile>
    {
        // add new methods specific to this repository
        Task<List<PricingProfile>> GetPricingProfilesByFarmIdAsync(Guid farmId, params string[] includeEntities);
    }
}
