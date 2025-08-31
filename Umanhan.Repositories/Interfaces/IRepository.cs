using System.Linq.Expressions;
using Umanhan.Models.Models;

namespace Umanhan.Repositories.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAllAsync(params string[] includeEntities);
        Task<T> GetByIdAsync(Guid id, params string[] includeEntities);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(Guid id);
    }
}
