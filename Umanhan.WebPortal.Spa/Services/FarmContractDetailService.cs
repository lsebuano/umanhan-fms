using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmContractDetailService
    {
        private readonly ApiService _apiService;

        public FarmContractDetailService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmContractDetailDto>>> GetFarmContractDetailsAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmContractDetailDto>>("OperationsAPI", $"api/farm-contracts/details/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmContractDetailDto>> GetFarmContractDetailByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmContractDetailDto> CreateFarmContractDetailAsync(FarmContractDetailDto farmContractDetail)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmContractDetailDto, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details", farmContractDetail).ConfigureAwait(false);
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

        public async Task<FarmContractDetailDto> UpdateFarmContractDetailAsync(FarmContractDetailDto farmContractDetail)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmContractDetailDto, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details", farmContractDetail).ConfigureAwait(false);
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

        public async Task<FarmContractDetailDto> DeleteFarmContractDetailAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/{id}").ConfigureAwait(false);
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

        public async Task<ApiResponse<FarmContractDetailDto>> ConfirmPickupAsync(Guid id)
        {
            try
            {
                return await _apiService.PutAsync<Guid, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/confirm-pickup/{id}", id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmContractDetailDto>> CancelTransactionAsync(Guid id)
        {
            try
            {
                return await _apiService.PutAsync<Guid, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/cancel-transaction/{id}", id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmContractDetailDto>> RecoverHarvestAsync(Guid id)
        {
            try
            {
                return await _apiService.PutAsync<Guid, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/recover-harvest/{id}", id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmContractDetailDto>> MarkTransactionAsPaidAsync(Guid id)
        {
            try
            {
                return await _apiService.PutAsync<Guid, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/mark-as-paid/{id}", id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmContractDetailDto>> SetHarvestDateAsyncAsync(Guid id, DateTime date)
        {
            try
            {
                return await _apiService.PutAsync<DateTime, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/set-harvest-date/{id}/{date:O}", date).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<ApiResponse<FarmContractDetailDto>> SetPickupDateAsyncAsync(Guid id, DateTime date)
        {
            try
            {
                return await _apiService.PutAsync<DateTime, FarmContractDetailDto>("OperationsAPI", $"api/farm-contracts/details/set-pickup-date/{id}/{date:O}", date).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
