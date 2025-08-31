using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class RoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<RoleService> _logger;

        private static List<RoleDto> ToRoleDto(IEnumerable<Role> roles)
        {
            return [.. roles.Select(x => new RoleDto
            {
                RoleId = x.Id,
                GroupName = x.Name,
                IsActive = x.IsActive,
                TemplateId = x.TemplateId,
                TemplateName = x.DashboardTemplate?.Name,
                DashboardTemplateComponentName = x.DashboardTemplate?.DashboardComponentName,
                RolePermissions = x.RolePermissions.Select(y => new RolePermissionDto{
                    ModuleId = y.ModuleId,
                    ModuleName = y.Module?.Name,
                    PermissionId = y.PermissionId,
                    PermissionName = y.Permission?.Name,
                    RoleId = y.RoleId,
                    RoleName = y.Role?.Name,
                    RoleIsActive = y.Role?.IsActive ?? false,
                    RolePermissionId = y.Id
                })
                .OrderBy(y => y.Access)
            })
            .OrderBy(x => x.GroupName)];
        }

        private static RoleDto ToRoleDto(Role role)
        {
            return new RoleDto
            {
                RoleId = role.Id,
                GroupName = role.Name,
                IsActive = role.IsActive,
                TemplateName = role.DashboardTemplate?.Name,
                DashboardTemplateComponentName = role.DashboardTemplate?.DashboardComponentName,
                RolePermissions = role.RolePermissions.Select(y => new RolePermissionDto
                {
                    ModuleId = y.ModuleId,
                    ModuleName = y.Module?.Name,
                    PermissionId = y.PermissionId,
                    PermissionName = y.Permission?.Name,
                    RoleId = y.RoleId,
                    RoleName = y.Role?.Name,
                    RoleIsActive = y.Role?.IsActive ?? false,
                    RolePermissionId = y.Id
                })
                .OrderBy(x => x.Access)
            };
        }

        public RoleService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<RoleService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(bool activeOnly = true, params string[] includeEntities)
        {
            IEnumerable<RoleDto> finalList = [];

            if (activeOnly)
                finalList = await _unitOfWork.Roles.GetAllActiveRolesAsync().ConfigureAwait(false);
            else
            {
                var list = await _unitOfWork.Roles.GetAllAsync(includeEntities).ConfigureAwait(false);
                finalList = ToRoleDto(list);
            }
            return finalList;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesExceptAsync(Func<Role, bool>? except = null, params string[] includeEntities)
        {
            IEnumerable<RoleDto> finalList = [];
            var list = await _unitOfWork.Roles.GetAllAsync(includeEntities).ConfigureAwait(false);
            if (except != null)
            {
                list = list.Where(x => !except(x)).ToList();
            }
            finalList = ToRoleDto(list);
            return finalList;
        }

        public async Task<RoleDto> GetRoleByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Roles.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToRoleDto(obj);
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto role)
        {
            var newRole = new Role
            {
                Name = role.GroupName,
                IsActive = role.IsActive,
                TemplateId = role.TemplateId,
            };

            try
            {
                var createdRole = await _unitOfWork.Roles.AddAsync(newRole).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToRoleDto(createdRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", role.GroupName);
                throw;
            }
        }

        public async Task<RoleDto> UpdateRoleAsync(RoleDto role)
        {
            var roleEntity = await _unitOfWork.Roles.GetByIdAsync(role.RoleId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Role not found.");
            roleEntity.Name = role.GroupName;
            roleEntity.IsActive = role.IsActive;
            roleEntity.TemplateId = role.TemplateId;

            try
            {
                var updatedRole = await _unitOfWork.Roles.UpdateAsync(roleEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToRoleDto(updatedRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleName}", role.GroupName);
                throw;
            }
        }

        public async Task<RoleDto> DeleteRoleAsync(Guid id)
        {
            var roleEntity = await _unitOfWork.Roles.GetByIdAsync(id).ConfigureAwait(false);
            if (roleEntity == null)
                return null;

            try
            {
                var deletedRole = await _unitOfWork.Roles.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToRoleDto(new Role());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID: {RoleId}", id);
                throw;
            }
        }
    }
}
