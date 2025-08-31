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

namespace Umanhan.Repositories
{
    public class ChangeLogRepository : UmanhanRepository<ChangeLog>, IChangeLogRepository
    {
        public ChangeLogRepository(UmanhanDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChangeLog>> GetLogsAsync(DateTime date)
        {
            return await _context.ChangeLogs.AsNoTracking()
                                            .Where(log => log.Timestamp.Month == date.Date.Month &&
                                                          log.Timestamp.Year == date.Date.Year)
                                            .ToListAsync();
        }
    }
}
