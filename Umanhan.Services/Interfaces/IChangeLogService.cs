using Umanhan.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface IChangeLogService
    {
        Task<IEnumerable<ChangeLogDto>> GetChangeLogsAsync(DateTime date);
        Task<ChangeLogDto> GetChangeLogByIdAsync(Guid id);
    }
}
