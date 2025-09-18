using Microsoft.EntityFrameworkCore;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.LoggerContext.Interfaces;
using Umanhan.Shared.Model;

namespace Umanhan.Repositories.LoggerContext
{
    public class QueryLogRepository : UmanhanLoggerRepository<EfQueryLog>, IQueryLogRepository
    {
        public QueryLogRepository(UmanhanLoggerDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<EfQueryLog>> GetLogsAsync(DateTime date, int pageNumber, int pageSize)
        {
            var query = _context.EfQueryLogs.AsNoTracking()
                .Where(log => log.CreatedAt != null &&
                              log.CreatedAt.Value.Month == date.Month &&
                              log.CreatedAt.Value.Year == date.Year);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(log => log.CreatedAt) // important for stable paging
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EfQueryLog>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
