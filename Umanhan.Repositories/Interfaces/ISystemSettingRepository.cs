using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface ISystemSettingRepository : IRepository<SystemSetting>
    {
        // add new methods specific to this repository
        Task<SystemSetting> GetSystemSettingByName(string settingName); 
    }
}
