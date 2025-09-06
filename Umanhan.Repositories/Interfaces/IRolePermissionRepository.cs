using Umanhan.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        // add new methods specific to this repository
        Task<List<RolePermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId, params string[] includeEntities);
        Task<List<RolePermissionDto>> GetPermissionsByRoleNameAsync(string roleName);
    }
}
