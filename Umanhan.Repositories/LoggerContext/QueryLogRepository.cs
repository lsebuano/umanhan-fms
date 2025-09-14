using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.LoggerContext;
using Umanhan.Repositories.LoggerContext.Interfaces;

namespace Umanhan.Repositories.LoggerContext
{
    public class QueryLogRepository : UmanhanLoggerRepository<EfQueryLog>, IQueryLogRepository
    {
        public QueryLogRepository(UmanhanLoggerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EfQueryLog>> GetLogsAsync(DateTime date)
        {
            return await _context.EfQueryLogs.AsNoTracking()
                                            .Where(log => log.CreatedAt != null &&
                                                          log.CreatedAt.Value.Month == date.Date.Month &&
                                                          log.CreatedAt.Value.Year == date.Date.Year)
                                            .ToListAsync();
        }
    }
}
