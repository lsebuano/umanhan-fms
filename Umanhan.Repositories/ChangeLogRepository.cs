using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Repositories.LoggerContext;
using Umanhan.Shared.Model;

namespace Umanhan.Repositories
{
    public class ChangeLogRepository : UmanhanRepository<ChangeLog>, IChangeLogRepository
    {
        public ChangeLogRepository(UmanhanDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<ChangeLog>> GetLogsAsync(DateTime date, int pageNumber, int pageSize)
        {
            var query = _context.ChangeLogs.AsNoTracking()
                .Where(log => log.Timestamp.Month == date.Date.Month &&
                              log.Timestamp.Year == date.Date.Year);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(log => log.Timestamp) // important for stable paging
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ChangeLog>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
