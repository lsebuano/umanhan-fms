using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class TaskService
    {
        private readonly ApiService _apiService;

        public TaskService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<TaskDto>>> GetAllTasksAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<TaskDto>>("MasterdataAPI", "api/tasks");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<TaskDto>>> GetTasksByCategoryAsync(Guid categoryId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<TaskDto>>("MasterdataAPI", $"api/tasks/getby/{categoryId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<TaskDto>>> GetTasksForFarmActivitiesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<TaskDto>>("MasterdataAPI", $"api/tasks/cat-group-activities");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<TaskDto>> GetTaskByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<TaskDto>("MasterdataAPI", $"api/tasks/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TaskDto> UnassignTaskFromCategoryAsync(Guid taskId)
        {
            try
            {
                var response = await _apiService.PutAsync<TaskDto, TaskDto>("MasterdataAPI", $"api/tasks/unassign", new TaskDto { TaskId = taskId }).ConfigureAwait(false);
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<TaskDto> CreateTaskAsync(TaskDto task)
        {
            try
            {
                var response = await _apiService.PostAsync<TaskDto, TaskDto>("MasterdataAPI", "api/tasks", task).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<TaskDto> UpdateTaskAsync(TaskDto task)
        {
            try
            {
                var response = await _apiService.PutAsync<TaskDto, TaskDto>("MasterdataAPI", $"api/tasks/{task.TaskId}", task).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<TaskDto> DeleteTaskAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<TaskDto>("MasterdataAPI", $"api/tasks/{id}").ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
