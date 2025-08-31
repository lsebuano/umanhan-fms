using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.LoggerContext.Interfaces;

namespace Umanhan.Repositories.LoggerContext
{
    public class LogRepository : UmanhanLoggerRepository<Log>, ILogRepository
    {
        public LogRepository(UmanhanLoggerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Log>> GetLogsAsync(Guid farmId, DateTime date)
        {
            return await _context.Logs.AsNoTracking()
                                      .Where(log => log.FarmId == farmId && 
                                                    log.Timestamp.Month == date.Date.Month &&
                                                    log.Timestamp.Year == date.Date.Year)
                                      .ToListAsync();
        }
    }
}
