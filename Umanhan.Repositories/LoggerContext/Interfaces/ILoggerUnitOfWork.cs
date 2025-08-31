using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories.LoggerContext.Interfaces
{
    public interface ILoggerUnitOfWork: IDisposable
    {
        //IChangeLogRepository ChangeLogs { get; }
        ILogRepository Logs { get; }
        IUserActivityRepository UserActivities { get; }

        Task<int> CommitAsync();
    }
}
