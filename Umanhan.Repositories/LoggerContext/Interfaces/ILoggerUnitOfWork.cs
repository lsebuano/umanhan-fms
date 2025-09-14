using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.LoggerContext.Interfaces
{
    public interface ILoggerUnitOfWork: IDisposable
    {
        //IChangeLogRepository ChangeLogs { get; }
        IQueryLogRepository QueryLogs { get; }
        ILogRepository Logs { get; }
        IUserActivityRepository UserActivities { get; }

        Task<int> CommitAsync();
    }
}
