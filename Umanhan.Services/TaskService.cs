using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class TaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<TaskService> _logger;

        private static List<TaskDto> ToTaskDto(IEnumerable<Models.Entities.Task> tasks)
        {
            return [.. tasks.Select(x => new TaskDto
            {
                TaskId = x.Id,
                TaskName = x.TaskName,
                CategoryId = x.CategoryId,
                Category = x.Category?.CategoryName,
                CategoryGroup = x.Category?.Group,
                CategoryGroup2 = x.Category?.Group2,
                CategoryConsumptionBehavior = x.Category?.ConsumptionBehavior,
            })
            .OrderBy(x => x.TaskName)];
        }

        private static TaskDto ToTaskDto(Models.Entities.Task task)
        {
            return new TaskDto
            {
                TaskId = task.Id,
                TaskName = task.TaskName,
                CategoryId = task.CategoryId,
                Category = task.Category?.CategoryName,
                CategoryGroup = task.Category?.Group,
                CategoryGroup2 = task.Category?.Group2,
                CategoryConsumptionBehavior = task.Category?.ConsumptionBehavior,
            };
        }

        public TaskService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<TaskService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<List<TaskDto>> GetAllTasksAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Tasks.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToTaskDto(list);
        }

        public async Task<List<TaskDto>> GetTasksByCategoryAsync(Guid categoryId, params string[] includeEntities)
        {
            return await _unitOfWork.Tasks.GetTasksByCategoryAsync(categoryId, includeEntities).ConfigureAwait(false);
        }

        public async Task<List<TaskDto>> GetTasksForFarmActivitiesAsync()
        {
            string group = "ACTIVITIES";
            return await _unitOfWork.Tasks.GetTasksForFarmActivitiesAsync(group).ConfigureAwait(false);
        }

        public async Task<TaskDto> GetTaskByIdAsync(Guid id, params string[] includeEntities)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            return ToTaskDto(task);
        }

        public async Task<TaskDto> CreateTaskAsync(TaskDto task)
        {
            var newTask = new Models.Entities.Task
            {
                TaskName = task.TaskName,
                CategoryId = task.CategoryId
            };

            try
            {
                var createdTask = await _unitOfWork.Tasks.AddAsync(newTask).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToTaskDto(createdTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task: {TaskName}", task.TaskName);
                throw;
            }
        }

        public async Task<TaskDto> UpdateTaskAsync(TaskDto task)
        {
            var taskEntity = await _unitOfWork.Tasks.GetByIdAsync(task.TaskId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Task not found");
            taskEntity.TaskName = task.TaskName;
            taskEntity.CategoryId = task.CategoryId;

            try
            {
                var updatedTask = await _unitOfWork.Tasks.UpdateAsync(taskEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToTaskDto(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task: {TaskName}", task.TaskName);
                throw;
            }
        }

        public async Task<TaskDto> DeleteTaskAsync(Guid id)
        {
            var taskEntity = await _unitOfWork.Tasks.GetByIdAsync(id).ConfigureAwait(false);
            if (taskEntity == null)
                return null;

            try
            {
                var obj = await _unitOfWork.Tasks.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToTaskDto(new Models.Entities.Task());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
                throw;
            }
        }
    }
}
