using Umanhan.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        // add new methods specific to this repository
        Task<List<RoleDto>> GetAllActiveRolesAsync();
    }
}
