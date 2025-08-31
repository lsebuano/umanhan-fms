using System.Diagnostics;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmGeneralExpenseService
    {
        private readonly ApiService _apiService;

        public FarmGeneralExpenseService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmGeneralExpenseDto>>> GetFarmGeneralExpensesAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmGeneralExpenseDto>>("OperationsAPI", $"api/farm-general-expenses/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmGeneralExpenseDto>>> GetFarmGeneralExpensesAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmGeneralExpenseDto>>("OperationsAPI", $"api/farm-general-expenses/farm-id/{farmId}/{startDate:O}/{endDate:O}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmGeneralExpenseDto>> GetFarmExpenseByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmGeneralExpenseDto>("OperationsAPI", $"api/farm-general-expenses/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResponse<FarmGeneralExpenseDto>> CreateFarmGeneralExpenseAsync(FarmGeneralExpenseDto farmExpense)
        {
            try
            {
                return await _apiService.PostAsync<FarmGeneralExpenseDto, FarmGeneralExpenseDto>("OperationsAPI", $"api/farm-general-expenses", farmExpense).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmGeneralExpenseDto>> UpdateFarmGeneralExpenseAsync(FarmGeneralExpenseDto farmExpense)
        {
            try
            {
                return await _apiService.PutAsync<FarmGeneralExpenseDto, FarmGeneralExpenseDto>("OperationsAPI", $"api/farm-general-expenses/{farmExpense.ExpenseId}", farmExpense).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmGeneralExpenseDto>> DeleteFarmGeneralExpenseAsync(Guid id)
        {
            try
            {
                return await _apiService.DeleteAsync<FarmGeneralExpenseDto>("OperationsAPI", $"api/farm-general-expenses/{id}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
