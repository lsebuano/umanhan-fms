using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ExpenseTypeService
    {
        private readonly ApiService _apiService;

        public ExpenseTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<ExpenseTypeDto>>> GetAllExpenseTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<ExpenseTypeDto>>("MasterdataAPI", "api/expense-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<ExpenseTypeDto>> GetExpenseTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<ExpenseTypeDto>("MasterdataAPI", $"api/expense-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ExpenseTypeDto> CreateExpenseTypeAsync(ExpenseTypeDto expenseType)
        {
            try
            {
                var response = await _apiService.PostAsync<ExpenseTypeDto, ExpenseTypeDto>("MasterdataAPI", "api/expense-types", expenseType).ConfigureAwait(false);
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

        public async Task<ExpenseTypeDto> UpdateExpenseTypeAsync(ExpenseTypeDto expenseType)
        {
            try
            {
                var response = await _apiService.PutAsync<ExpenseTypeDto, ExpenseTypeDto>("MasterdataAPI", $"api/expense-types/{expenseType.TypeId}", expenseType).ConfigureAwait(false);
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

        public async Task<ExpenseTypeDto> DeleteExpenseTypeAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ExpenseTypeDto>("MasterdataAPI", $"api/expense-types/{id}").ConfigureAwait(false);
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
