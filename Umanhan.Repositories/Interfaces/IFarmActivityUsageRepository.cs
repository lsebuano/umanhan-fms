using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmActivityUsageRepository : IRepository<FarmActivityUsage>
    {

        // add new methods specific to this repository
        Task<List<FarmActivityUsage>> GetFarmActivityUsagesAsync(Guid farmId, string[] includeEntities);
        Task<List<FarmActivityUsage>> GetFarmActivityUsagesByActivityAsync(Guid activityId, string[] includeEntities);
        Task<List<FarmActivityUsage>> GetFarmActivityUsagesByItemAsync(Guid farmId, Guid itemId, string[] includeEntities);
    }
}
