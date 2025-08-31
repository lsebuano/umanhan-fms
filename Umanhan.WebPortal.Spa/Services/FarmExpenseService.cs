using System.Diagnostics;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmExpenseService
    {
        private readonly ApiService _apiService;

        public FarmExpenseService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmActivityExpenseDto>>> GetFarmActivityExpensesAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityExpenseDto>>("OperationsAPI", $"api/farm-expenses/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmActivityExpenseDto>> GetFarmExpenseByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmActivityExpenseDto>("OperationsAPI", $"api/farm-expenses/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmActivityExpenseDto>>> GetFarmExpenseByActivityAsync(Guid activityId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityExpenseDto>>("OperationsAPI", $"api/farm-expenses/activity/{activityId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmActivityExpenseDto> CreateFarmActivitiesAsync(FarmActivityExpenseDto farmExpense)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmActivityExpenseDto, FarmActivityExpenseDto>("OperationsAPI", $"api/farm-expenses", farmExpense).ConfigureAwait(false);
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

        public async Task<FarmActivityExpenseDto> UpdateFarmActivitiesAsync(FarmActivityExpenseDto farmExpense)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmActivityExpenseDto, FarmActivityExpenseDto>("OperationsAPI", $"api/farm-expenses/{farmExpense.ExpenseId}", farmExpense).ConfigureAwait(false);
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

        public async Task<FarmActivityExpenseDto> DeleteFarmActivitiesAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmActivityExpenseDto>("OperationsAPI", $"api/farm-expenses/{id}").ConfigureAwait(false);
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
