using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories.LoggerContext
{
    public class UmanhanLoggerRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly UmanhanLoggerDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public UmanhanLoggerRepository(UmanhanLoggerDbContext context)
        {
            this._context = context;
            this._dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity).ConfigureAwait(false);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        public Task<T> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetAllAsync(params string[] includeEntities)
        {
            try
            {
                if (includeEntities.Any())
                {
                    var query = _dbSet.AsQueryable();
                    foreach (var includeEntity in includeEntities)
                    {
                        query = query.Include(includeEntity);
                    }
                    return await query.AsNoTracking()
                                      .AsSplitQuery()
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                }

                return await _dbSet.AsNoTracking()
                                   .ToListAsync()
                                   .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        public async Task<T> GetByIdAsync(Guid id, params string[] includeEntities)
        {
            try
            {
                if (includeEntities.Any())
                {
                    var query = _dbSet.AsQueryable().AsSplitQuery();
                    foreach (var includeEntity in includeEntities)
                    {
                        query = query.Include(includeEntity);
                    }
                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Id == id)
                                      .ConfigureAwait(false);
                }
                return await _dbSet.FindAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        public Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
