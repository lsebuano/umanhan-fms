namespace Umanhan.WebPortal.Spa.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string permission);
        Task<IEnumerable<string>> GetPermissions();
    }
}
