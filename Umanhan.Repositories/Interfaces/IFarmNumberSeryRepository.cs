using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmNumberSeryRepository : IRepository<FarmNumberSery>
    {
        // add new methods specific to this repository
        Task<string> GenerateNumberSeryAsync(Guid farmId, string type);
    }
}
