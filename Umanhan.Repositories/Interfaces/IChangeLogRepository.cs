using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Shared.Model;

namespace Umanhan.Repositories.Interfaces
{
    public interface IChangeLogRepository : IRepository<ChangeLog>
    {
        // add new methods specific to this repository
        Task<PagedResult<ChangeLog>> GetLogsAsync(DateTime date, int pageNumber, int pageSize);
    }
}
