using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class SystemSettingRepository : UmanhanRepository<SystemSetting>, ISystemSettingRepository
    {
        public SystemSettingRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository
        public Task<SystemSetting> GetSystemSettingByName(string settingName)
        {
            return _context.SystemSettings.AsNoTracking()
                                          .Where(x => x.SettingName.Equals(settingName))
                                          .Select(x => new SystemSetting
                                          {
                                              Id = x.Id,
                                              SettingName = x.SettingName,
                                              SettingValue = x.SettingValue
                                          })
                                          .FirstOrDefaultAsync();
        }
    }
}
