using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmActivityPhotoRepository : IRepository<FarmActivityPhoto>
    {
        // add new methods specific to this repository
        Task<List<FarmActivityPhoto>> GetFarmActivityPhotosByActivityAsync(Guid activityId, params string[] includeEntities);
    }
}
