using Microsoft.EntityFrameworkCore;
using Umanhan.Dtos;
using Umanhan.Models;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class TaskRepository : UmanhanRepository<Models.Entities.Task>, ITaskRepository
    {
        public TaskRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<TaskDto>> GetTasksByCategoryAsync(Guid categoryId, params string[] includeEntities)
        {
            if (includeEntities.Any())
            {
                var query = _dbSet.AsQueryable().AsSplitQuery();
                foreach (var includeEntity in includeEntities)
                {
                    query = query.Include(includeEntity);
                }
                return query.AsNoTracking()
                            .Where(x => x.CategoryId == categoryId)
                            .Select(x => new TaskDto
                            {
                                TaskId = x.Id,
                                TaskName = x.TaskName,
                                CategoryId = x.CategoryId,
                                CategoryGroup = x.Category.Group,
                                CategoryGroup2 = x.Category.Group2,
                                CategoryConsumptionBehavior = x.Category.ConsumptionBehavior,
                            })
                            .ToListAsync();
            }

            return _context.Tasks.AsNoTracking()
                                 .Where(x => x.CategoryId.Equals(categoryId))
                                 .Select(x => new TaskDto
                                 {
                                     TaskId = x.Id,
                                     TaskName = x.TaskName,
                                     CategoryId = x.CategoryId,
                                     CategoryGroup = x.Category.Group,
                                     CategoryGroup2 = x.Category.Group2,
                                     CategoryConsumptionBehavior = x.Category.ConsumptionBehavior,
                                 })
                                 .ToListAsync();
        }

        public Task<List<TaskDto>> GetTasksForFarmActivitiesAsync(string group)
        {
            return _context.Tasks.AsNoTracking()
                                 .AsSplitQuery()
                                 .Include(x => x.Category)
                                 .Where(x => x.Category.Group == group)
                                 .Select(x => new TaskDto
                                 {
                                     TaskId = x.Id,
                                     TaskName = x.TaskName,
                                     CategoryId = x.CategoryId,
                                     CategoryGroup = x.Category.Group,
                                     CategoryGroup2 = x.Category.Group2,
                                     CategoryConsumptionBehavior = x.Category.ConsumptionBehavior,
                                 })
                                 .OrderBy(x => x.TaskName)
                                 .ToListAsync();
        }
    }
}
