using Umanhan.Dtos;
using Umanhan.Shared.Model;

namespace Umanhan.Services.Interfaces
{
    public interface IChangeLogService
    {
        Task<PagedResult<ChangeLogDto>> GetChangeLogsAsync(DateTime date, int pageNumber = 1, int pageSize = 20);
        Task<ChangeLogDto> GetChangeLogByIdAsync(Guid id);
    }
}
