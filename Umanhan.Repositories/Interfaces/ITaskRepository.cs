using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface ITaskRepository: IRepository<Models.Entities.Task>
    {
        // add new methods specific to this repository
        Task<List<TaskDto>> GetTasksByCategoryAsync(Guid categoryId, params string[] includeEntities);
        Task<List<TaskDto>> GetTasksForFarmActivitiesAsync(string group);
    }
}
