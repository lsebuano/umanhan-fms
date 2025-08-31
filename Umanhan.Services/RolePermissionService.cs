using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class RolePermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<RolePermissionService> _logger;

        private static List<RolePermissionDto> ToRolePermissionDto(IEnumerable<RolePermission> rolePermissions)
        {
            return [.. rolePermissions.Select(x => new RolePermissionDto
            {
                RolePermissionId = x.Id,
                ModuleId = x.ModuleId,
                PermissionId = x.PermissionId,
                RoleId = x.RoleId,
                ModuleName = x.Module?.Name,
                PermissionName = x.Permission?.Name,
                RoleIsActive = x.Role?.IsActive ?? false,
                RoleName = x.Role?.Name,
            })
            .OrderBy(x => x.RoleName)];
        }

        private static RolePermissionDto ToRolePermissionDto(RolePermission rolePermission)
        {
            return new RolePermissionDto
            {
                RolePermissionId = rolePermission.Id,
                ModuleId = rolePermission.ModuleId,
                PermissionId = rolePermission.PermissionId,
                RoleId = rolePermission.RoleId,
                ModuleName = rolePermission.Module?.Name,
                PermissionName = rolePermission.Permission?.Name,
                RoleIsActive = rolePermission.Role?.IsActive ?? false,
                RoleName = rolePermission.Role?.Name,
            };
        }

        public RolePermissionService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<RolePermissionService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllRolePermissionsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.RolePermissions.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToRolePermissionDto(list);
        }

        public async Task<RolePermissionDto> GetRolePermissionByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.RolePermissions.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToRolePermissionDto(obj);
        }

        public async Task<RolePermissionDto> CreateRolePermissionAsync(RolePermissionDto rolePermission)
        {
            var newRolePermission = new RolePermission
            {
                ModuleId = rolePermission.ModuleId,
                PermissionId = rolePermission.PermissionId,
                RoleId = rolePermission.RoleId
            };

            try
            {
                var createdRolePermission = await _unitOfWork.RolePermissions.AddAsync(newRolePermission).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToRolePermissionDto(createdRolePermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role permission");
                throw;
            }
        }

        public async Task<RolePermissionDto> UpdateRolePermissionAsync(RolePermissionDto rolePermission)
        {
            var rolePermissionEntity = await _unitOfWork.RolePermissions.GetByIdAsync(rolePermission.RolePermissionId).ConfigureAwait(false) ?? throw new KeyNotFoundException("RolePermission not found.");
            rolePermissionEntity.PermissionId = rolePermission.PermissionId;
            rolePermissionEntity.ModuleId = rolePermission.ModuleId;
            rolePermissionEntity.RoleId = rolePermission.RoleId;

            try
            {
                var updatedRolePermission = await _unitOfWork.RolePermissions.UpdateAsync(rolePermissionEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToRolePermissionDto(updatedRolePermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role permission: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<RolePermissionDto> DeleteRolePermissionAsync(Guid id)
        {
            var rolePermissionEntity = await _unitOfWork.RolePermissions.GetByIdAsync(id).ConfigureAwait(false);
            if (rolePermissionEntity == null)
                return null;

            try
            {
                var deletedRolePermission = await _unitOfWork.RolePermissions.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToRolePermissionDto(new RolePermission());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role permission: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<RolePermissionDto>> GetPermissionsForRoleAsync(Guid roleId, params string[] includeEntities)
        {
            try
            {
                var rolePermissions = await _unitOfWork.RolePermissions.GetPermissionsByRoleIdAsync(roleId, includeEntities).ConfigureAwait(false);
                return rolePermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for role with ID {RoleId}: {Message}", roleId, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<RolePermissionDto>> GetPermissionsForRoleAsync(string roleName)
        {
            try
            {
                var rolePermissions = await _unitOfWork.RolePermissions.GetPermissionsByRoleNameAsync(roleName).ConfigureAwait(false);
                return rolePermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for role with name {RoleName}: {Message}", roleName, ex.Message);
                throw;
            }
        }
    }
}
