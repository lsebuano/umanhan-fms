using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Repositories.LoggerContext.Interfaces;

namespace Umanhan.Repositories.LoggerContext
{
    public class LoggerUnitOfWork : ILoggerUnitOfWork
    {
        private readonly UmanhanLoggerDbContext _context;
        //public IChangeLogRepository ChangeLogs { get; private set; }
        public ILogRepository Logs { get; private set; }
        public IUserActivityRepository UserActivities { get; private set; }

        public LoggerUnitOfWork(UmanhanLoggerDbContext context)
        {
            _context = context;
            //ChangeLogs = new ChangeLogRepository(_context);
            Logs = new LogRepository(_context);
            UserActivities = new UserActivityRepository(_context);
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as PostgresException;
                if (sqlException != null)
                {
                    switch (sqlException.SqlState)
                    {
                        case "23505":
                            throw new DbUpdateException("Record already exists.");

                        case "42703":
                        case "42P18":
                            throw new DbUpdateException("Invalid column reference.");

                        case "23503":
                            throw new DbUpdateException("Foreign key constraint violated.");

                        case "42P01":
                            throw new DbUpdateException("Table not found.");

                        case "23514":
                            throw new DbUpdateException("Check constraint violation.");

                        default:
                            throw new DbUpdateException("Unknown database error.");
                    }
                }
                throw new DbUpdateException("Unexpected error. Please try again later.");
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
