using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PermissionService> _logger;
        private readonly RolePermissionService _rolePermissionService;
        private readonly MemoryCacheEntryOptions _cacheOptions;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new(StringComparer.Ordinal);

        // The hierarchy: Read=1, Write=2, Full=3
        private static readonly Dictionary<string, int> _levelRank = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Read"] = 1,
            ["Write"] = 2,
            ["Full"] = 3
        };

        private static List<PermissionDto> ToPermissionDto(IEnumerable<Permission> permissions)
        {
            return [.. permissions.Select(x => new PermissionDto
            {
                PermissionId = x.Id,
                Name = x.Name,
            })
            .OrderBy(x => x.Name)];
        }

        private static PermissionDto ToPermissionDto(Permission permission)
        {
            return new PermissionDto
            {
                PermissionId = permission.Id,
                Name = permission.Name,
            };
        }

        private static bool CheckPermissionInList(List<string> permissions, string requiredModule, int requiredRank)
        {
            if (permissions == null || permissions.Count == 0)
                return false;

            foreach (var perm in permissions)
            {
                var parts = perm.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                var module = parts[0];
                var level = parts[1];

                if (!string.Equals(module, requiredModule, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!_levelRank.TryGetValue(level, out var permRank))
                    continue;

                if (permRank >= requiredRank)
                    return true;
            }

            return false;
        }

        public PermissionService(IUnitOfWork unitOfWork, 
            IUserContextService userContext, 
            RolePermissionService rolePermissionService, 
            IMemoryCache cache,
            ILogger<PermissionService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _rolePermissionService = rolePermissionService;
            _cache = cache;
            this._logger = logger;

            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1), // Cache for 1 minute
                SlidingExpiration = TimeSpan.FromMinutes(5) // Reset expiration if accessed within 5 minutes
            };
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Permissions.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToPermissionDto(list);
        }

        public async Task<PermissionDto> GetPermissionByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Permissions.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToPermissionDto(obj);
        }

        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto Permission)
        {
            var newPermission = new Permission
            {
                Name = Permission.Name
            };

            try
            {
                var createdPermission = await _unitOfWork.Permissions.AddAsync(newPermission).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPermissionDto(createdPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission: {PermissionName}", Permission.Name);
                throw;
            }
        }

        public async Task<PermissionDto> UpdatePermissionAsync(PermissionDto permission)
        {
            var permissionEntity = await _unitOfWork.Permissions.GetByIdAsync(permission.PermissionId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Permission not found.");
            permissionEntity.Name = permission.Name;

            try
            {
                var updatedPermission = await _unitOfWork.Permissions.UpdateAsync(permissionEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPermissionDto(updatedPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission: {PermissionName}", permission.Name);
                throw;
            }
        }

        public async Task<PermissionDto> DeletePermissionAsync(Guid id)
        {
            var permissionEntity = await _unitOfWork.Permissions.GetByIdAsync(id).ConfigureAwait(false);
            if (permissionEntity == null)
                return null;

            try
            {
                var deletedPermission = await _unitOfWork.Permissions.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPermissionDto(new Permission());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission with ID: {PermissionId}", id);
                throw;
            }
        }

        public async Task<bool> HasPermissionAsync(IEnumerable<string> roleNames, string requiredPermission, bool forceCacheRefresh = false)
        {
            if (roleNames == null)
                return false;

            var parts = requiredPermission.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false; // invalid format

            var requiredModule = parts[0];  // e.g. "Farm"
            var requiredLevel = parts[1];   // e.g. "Write"
            if (!_levelRank.TryGetValue(requiredLevel, out var requiredRank))
                return false; // unknown level

            var cacheKey = $"PermissionService:HasPermission:{string.Join("_", roleNames)}";
            if (forceCacheRefresh)
            {
                _cache.Remove(cacheKey);
            }

            if (_cache.TryGetValue(cacheKey, out List<string> cachedPermissions))
            {
                return CheckPermissionInList(cachedPermissions, requiredModule, requiredRank);
            }

            // else
            // Fetch all permissions for the roles

            var semaphore = _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
            var lockTaken = false;

            try
            {
                lockTaken = await semaphore.WaitAsync(millisecondsTimeout: 3000);
                if (lockTaken)
                {
                    if (_cache.TryGetValue(cacheKey, out cachedPermissions))
                    {
                        return CheckPermissionInList(cachedPermissions, requiredModule, requiredRank);
                    }

                    var aggregatedPermissions = new List<string>();
                    foreach (var role in roleNames)
                    {
                        var perms = await _rolePermissionService.GetPermissionsForRoleAsync(role).ConfigureAwait(false);
                        if (perms.Any())
                            aggregatedPermissions.AddRange(perms.Select(x => x.Access).Distinct());
                    }

                    var distinctPermissions = aggregatedPermissions.Where(p => !string.IsNullOrWhiteSpace(p))
                                                                   .Distinct(StringComparer.OrdinalIgnoreCase)
                                                                   .ToList();

                    // store in cache
                    _cache.Set(cacheKey, distinctPermissions, _cacheOptions);

                    return CheckPermissionInList(distinctPermissions, requiredModule, requiredRank);
                }
                else
                {
                    // try read once more
                    if (_cache.TryGetValue(cacheKey, out cachedPermissions))
                    {
                        return CheckPermissionInList(cachedPermissions, requiredModule, requiredRank);
                    }
                    // fallback
                    return false;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    semaphore.Release(); // Release the lock if it was acquired

                    if (semaphore.CurrentCount == 1)
                    {
                        _locks.TryRemove(cacheKey, out _); // Remove the lock if it was the last one
                    }
                }
            }
        }
    }
}
