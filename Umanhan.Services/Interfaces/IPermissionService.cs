using Umanhan.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync(params string[] includeEntities);
        Task<PermissionDto> GetPermissionByIdAsync(Guid id, params string[] includeEntities);
        Task<PermissionDto> CreatePermissionAsync(PermissionDto Permission);
        Task<PermissionDto> UpdatePermissionAsync(PermissionDto permission);
        Task<PermissionDto> DeletePermissionAsync(Guid id);
        Task<bool> HasPermissionAsync(IEnumerable<string> roleNames, string requiredPermission, bool forceCacheRefresh = false);
    }
}
