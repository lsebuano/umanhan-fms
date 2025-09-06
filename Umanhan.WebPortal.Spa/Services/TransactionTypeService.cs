using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class TransactionTypeService
    {
        private readonly ApiService _apiService;

        public TransactionTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<TransactionTypeDto>>> GetAllTransactionTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<TransactionTypeDto>>("MasterdataAPI", "api/transaction-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<TransactionTypeDto>> GetTransactionTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<TransactionTypeDto>("MasterdataAPI", $"api/transaction-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TransactionTypeDto> CreateTransactionTypeAsync(TransactionTypeDto transactionType)
        {
            try
            {
                var response = await _apiService.PostAsync<TransactionTypeDto, TransactionTypeDto>("MasterdataAPI", "api/transaction-types", transactionType).ConfigureAwait(false);
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

        public async Task<TransactionTypeDto> UpdateTransactionTypeAsync(TransactionTypeDto transactionType)
        {
            try
            {
                var response = await _apiService.PutAsync<TransactionTypeDto, TransactionTypeDto>("MasterdataAPI", $"api/transaction-types/{transactionType.TypeId}", transactionType).ConfigureAwait(false);
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

        public async Task<TransactionTypeDto> DeleteTransactionTypeAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<TransactionTypeDto>("MasterdataAPI", $"api/transaction-types/{id}").ConfigureAwait(false);
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
