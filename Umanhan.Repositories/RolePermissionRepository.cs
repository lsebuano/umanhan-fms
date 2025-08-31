using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class RolePermissionRepository : UmanhanRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<RolePermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId, params string[] includeEntities)
        {
            IQueryable<RolePermission> query = _context.RolePermissions.AsNoTracking();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.AsSplitQuery()
                        .Where(x => x.RoleId == roleId)
                        .Select(x => new RolePermissionDto
                        {
                            RoleId = x.RoleId,
                            RolePermissionId = x.Id,
                            RoleName = x.Role.Name,
                            RoleIsActive = x.Role.IsActive,
                            ModuleId = x.ModuleId,
                            ModuleName = x.Module.Name,
                            PermissionId = x.PermissionId,
                            PermissionName = x.Permission.Name
                        })
                        .ToListAsync();
        }

        public Task<List<RolePermissionDto>> GetPermissionsByRoleNameAsync(string roleName)
        {
            IQueryable<RolePermission> query = _context.RolePermissions.AsNoTracking().AsSplitQuery();
            return query.Include(x => x.Role)
                        .Where(x => x.Role.Name == roleName)
                        .Select(x => new RolePermissionDto
                        {
                            RoleId = x.RoleId,
                            RolePermissionId = x.Id,
                            RoleName = x.Role.Name,
                            RoleIsActive = x.Role.IsActive,
                            ModuleId = x.ModuleId,
                            ModuleName = x.Module.Name,
                            PermissionId = x.PermissionId,
                            PermissionName = x.Permission.Name
                        })
                        .ToListAsync();
        }
    }
}
