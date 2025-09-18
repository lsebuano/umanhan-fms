using Umanhan.Dtos;
using Umanhan.Shared.Model;

namespace Umanhan.Services.Interfaces
{
    public interface IQueryLogService
    {
        Task<PagedResult<QueryLogDto>> GetQueryLogsAsync(DateTime date, int pageNumber = 1, int pageSize = 20);
        Task<QueryLogDto> GetQueryLogByIdAsync(Guid id);
    }
}
