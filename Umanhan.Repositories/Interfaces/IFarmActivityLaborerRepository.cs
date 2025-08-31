using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmActivityLaborerRepository : IRepository<FarmActivityLaborer>
    {
        // add new methods specific to this repository
        Task<List<FarmActivityLaborer>> GetFarmActivityLaborersAsync(Guid farmId, string[] includeEntities);
        Task<List<FarmActivityLaborer>> GetFarmActivityLaborersByActivityAsync(Guid activityId, params string[] includeEntities);
    }
}
