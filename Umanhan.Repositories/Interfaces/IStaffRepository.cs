using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IStaffRepository : IRepository<Staff>
    {
        // add new methods specific to this repository
        Task<List<Staff>> GetStaffsByFarmAsync(Guid farmId);
    }
}
