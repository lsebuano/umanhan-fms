using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories.LoggerContext.Interfaces
{
    public interface IUserActivityRepository : IRepository<UserActivity>
    {
        // add new methods specific to this repository
        Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid farmId, DateTime date);
        Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid farmId, string username);
    }
}
