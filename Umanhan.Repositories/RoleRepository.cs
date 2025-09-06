using Microsoft.EntityFrameworkCore;
using Umanhan.Dtos;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class RoleRepository : UmanhanRepository<Role>, IRoleRepository
    {
        public RoleRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<RoleDto>> GetAllActiveRolesAsync()
        {
            return _context.Roles.AsNoTracking()
                                 .AsSplitQuery()
                                 .Include(x => x.RolePermissions)
                                 .Include(x => x.DashboardTemplate)
                                 .Where(x => x.IsActive)
                                 .Select(x => new RoleDto
                                 {
                                     RoleId = x.Id,
                                     GroupName = x.Name,
                                     IsActive = x.IsActive,
                                     DashboardTemplateComponentName = x.DashboardTemplate.DashboardComponentName,
                                     RolePermissions = x.RolePermissions.Select(y => new RolePermissionDto {
                                         ModuleId = y.ModuleId,
                                         PermissionId = y.PermissionId,
                                         ModuleName = y.Module.Name,
                                         PermissionName = y.Permission.Name,
                                         RoleId = y.RoleId,
                                         RoleIsActive = x.IsActive,
                                         RoleName = x.Name,
                                         RolePermissionId = y.Id
                                     })
                                 })
                                 .ToListAsync();
        }
    }
}
