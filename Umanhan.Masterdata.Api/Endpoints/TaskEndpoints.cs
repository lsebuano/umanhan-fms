using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class TaskEndpoints
    {
        private readonly TaskService _taskService;
        private readonly IValidator<TaskDto> _validator;
        private readonly ILogger<TaskEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "task";
        private const string MODULE_CACHE_TAG = "task:list:all";

        public TaskEndpoints(TaskService taskService, IValidator<TaskDto> validator, ILogger<TaskEndpoints> logger)
        {
            _taskService = taskService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllTasksAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _taskService.GetAllTasksAsync("Category").ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                string msg = "Error retrieving all tasks";
                _logger.LogError(ex, msg);
                return Results.Problem(msg);
            }
        }

        public async Task<IResult> GetTasksByCategoryAsync(Guid categoryId)
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list:{categoryId}";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _taskService.GetTasksByCategoryAsync(categoryId, "Category").ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                string msg = $"Error retrieving tasks for category with ID {categoryId}";
                _logger.LogError(ex, msg);
                return Results.Problem(msg);
            }
        }

        public async Task<IResult> GetTasksForFarmActivitiesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list:activities";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _taskService.GetTasksForFarmActivitiesAsync().ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                string msg = "Error retrieving tasks for farm activities";
                _logger.LogError(ex, msg);
                return Results.Problem(msg);
            }
        }

        public async Task<IResult> GetTaskByIdAsync(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id).ConfigureAwait(false);
                return task is not null ? Results.Ok(task) : Results.NotFound();
            }
            catch (Exception ex)
            {
                string msg = $"Error retrieving task with ID {id}";
                _logger.LogError(ex, msg);
                return Results.Problem(msg);
            }
        }

        public async Task<IResult> CreateTaskAsync(TaskDto task)
        {
            var validationResult = await _validator.ValidateAsync(task).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newTask = await _taskService.CreateTaskAsync(task).ConfigureAwait(false);

                //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);

                return Results.Created($"/api/tasks/{newTask.TaskId}", newTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateTaskAsync(Guid id, TaskDto task)
        {
            if (id != task.TaskId)
                return Results.BadRequest("Task ID mismatch");

            var validationResult = await _validator.ValidateAsync(task).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(task).ConfigureAwait(false);
                if (updatedTask is not null)
                {
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    return Results.Ok(updatedTask);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID {TaskId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteTaskAsync(Guid id)
        {
            try
            {
                var deletedTask = await _taskService.DeleteTaskAsync(id).ConfigureAwait(false);
                if (deletedTask is not null)
                {
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    return Results.Ok(deletedTask);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UnassignTaskToCategoryAsync(TaskDto data)
        {
            try
            {
                var id = data.TaskId;
                var task = await _taskService.GetTaskByIdAsync(id).ConfigureAwait(false);
                if (task is null)
                {
                    return Results.NotFound();
                }

                task.CategoryId = null;
                var updatedTask = await _taskService.UpdateTaskAsync(task).ConfigureAwait(false);
                if (updatedTask is not null)
                {
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    return Results.Ok(updatedTask);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unassigning task with ID {TaskId} from category", data.TaskId);
                return Results.Problem(ex.Message);
            }
        }
    }
}