using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmContractService
    {
        private readonly ApiService _apiService;

        public FarmContractService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmContractDto>>> GetFarmContractsAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmContractDto>>("OperationsAPI", $"api/farm-contracts/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmContractDto>>> GetFarmContractsAsync(Guid farmId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmContractDto>>("OperationsAPI", $"api/farm-contracts/farm-id/{farmId}/{startDate:O}/{endDate:O}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmContractDto>> GetFarmContractByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmContractDto>("OperationsAPI", $"api/farm-contracts/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmContractDto> CreateFarmContractAsync(FarmContractDto farmContract)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmContractDto, FarmContractDto>("OperationsAPI", $"api/farm-contracts", farmContract).ConfigureAwait(false);
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

        public async Task<FarmContractDto> UpdateFarmContractAsync(FarmContractDto farmContract)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmContractDto, FarmContractDto>("OperationsAPI", $"api/farm-contracts", farmContract).ConfigureAwait(false);
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

        public async Task<FarmContractDto> DeleteFarmContractAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmContractDto>("OperationsAPI", $"api/farm-contracts/{id}").ConfigureAwait(false);
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
