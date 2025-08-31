using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmNumberSeryRepository : IRepository<FarmNumberSery>
    {
        // add new methods specific to this repository
        Task<string> GenerateNumberSeryAsync(Guid farmId, string type);
    }
}
