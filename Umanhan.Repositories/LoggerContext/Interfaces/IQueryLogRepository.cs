using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Shared.Model;

namespace Umanhan.Repositories.LoggerContext.Interfaces
{
    public interface IQueryLogRepository : IRepository<EfQueryLog>
    {
        // add new methods specific to this repository
        Task<PagedResult<EfQueryLog>> GetLogsAsync(DateTime date, int pageNumber = 1, int pageSize = 20);
    }
}
