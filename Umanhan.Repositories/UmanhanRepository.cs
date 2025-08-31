using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class UmanhanRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly UmanhanDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public UmanhanRepository(UmanhanDbContext context)
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
                    return await query.AsSplitQuery()
                                      .AsNoTracking()
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
                var query = _dbSet.AsQueryable();
                if (includeEntities.Length != 0)
                {
                    foreach (var includeEntity in includeEntities)
                    {
                        query = query.Include(includeEntity);
                    }
                }
                return await query.AsSplitQuery()
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(x => x.Id == id)
                                  .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        public async Task<T> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id).ConfigureAwait(false);
                if (entity == null)
                {
                    return entity;
                }

                _dbSet.Remove(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }
    }
}
