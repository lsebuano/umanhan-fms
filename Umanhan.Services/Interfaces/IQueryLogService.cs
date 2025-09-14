using Umanhan.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface IQueryLogService
    {
        Task<IEnumerable<QueryLogDto>> GetQueryLogsAsync(DateTime date);
        Task<QueryLogDto> GetQueryLogByIdAsync(Guid id);
    }
}
