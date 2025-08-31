using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.LoggerContext.Interfaces;

namespace Umanhan.Repositories.LoggerContext
{
    public class UserActivityRepository : UmanhanLoggerRepository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(UmanhanLoggerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid farmId, DateTime date)
        {
            return await _context.UserActivities.AsNoTracking()
                                                .Where(x => x.FarmId == farmId &&
                                                            x.Timestamp.Month == date.Month &&
                                                            x.Timestamp.Year == date.Year)
                                                .ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid farmId, string username)
        {
            return await _context.UserActivities.AsNoTracking()
                                                .Where(x => x.FarmId == farmId &&
                                                            x.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                                                .ToListAsync();
        }
    }
}
