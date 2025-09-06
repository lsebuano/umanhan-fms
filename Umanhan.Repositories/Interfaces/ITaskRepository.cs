using Umanhan.Dtos;

namespace Umanhan.Repositories.Interfaces
{
    public interface ITaskRepository: IRepository<Models.Entities.Task>
    {
        // add new methods specific to this repository
        Task<List<TaskDto>> GetTasksByCategoryAsync(Guid categoryId, params string[] includeEntities);
        Task<List<TaskDto>> GetTasksForFarmActivitiesAsync(string group);
    }
}
