using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmTransactionService
    {
        private readonly ApiService _apiService;

        public FarmTransactionService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmTransactionDto>>> GetFarmTransactionsAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmTransactionDto>>("OperationsAPI", $"api/farm-sales/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmTransactionDto>>> GetRecentFarmTransactionsAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmTransactionDto>>("OperationsAPI", $"api/farm-sales/recent/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmTransactionDto>> GetFarmTransactionByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmTransactionDto>("OperationsAPI", $"api/farm-sales/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmTransactionDto> CreateFarmTransactionAsync(FarmTransactionDto farmTransaction)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmTransactionDto, FarmTransactionDto>("OperationsAPI", $"api/farm-sales", farmTransaction).ConfigureAwait(false);
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

        public async Task<FarmTransactionDto> UpdateFarmTransactionAsync(FarmTransactionDto farmTransaction)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmTransactionDto, FarmTransactionDto>("OperationsAPI", $"api/farm-sales", farmTransaction).ConfigureAwait(false);
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

        public async Task<FarmTransactionDto> DeleteFarmTransactionAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmTransactionDto>("OperationsAPI", $"api/farm-sales/{id}").ConfigureAwait(false);
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

        public async Task<ApiResponse<FarmTransactionDto>> CancelTransactionAsync(Guid id)
        {
            try
            {
                return await _apiService.PutAsync<Guid, FarmTransactionDto>("OperationsAPI", $"api/farm-sales/cancel-transaction/{id}", id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
