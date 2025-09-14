using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories.LoggerContext.Interfaces
{
    public interface IQueryLogRepository : IRepository<EfQueryLog>
    {
        // add new methods specific to this repository
        Task<IEnumerable<EfQueryLog>> GetLogsAsync(DateTime date);
    }
}
