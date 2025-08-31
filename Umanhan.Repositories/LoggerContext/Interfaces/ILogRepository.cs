using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories.LoggerContext.Interfaces
{
    public interface ILogRepository : IRepository<Log>
    {
        // add new methods specific to this repository
        Task<IEnumerable<Log>> GetLogsAsync(Guid farmId, DateTime date);
    }
}
