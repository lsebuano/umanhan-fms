using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class PricingService
    {
        private readonly ApiService _apiService;

        public PricingService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<PricingDto>>> GetAllPricingsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<PricingDto>>("OperationsAPI", "api/pricing");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<PricingDto>> GetPricingByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<PricingDto>("OperationsAPI", $"api/pricing/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<PricingDto>>> GetPricingsByFarmIdAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<PricingDto>>("OperationsAPI", $"api/pricing/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<PricingResult>> CalculateFinalPriceAsync(Guid profileId, decimal basePrice)
        {
            try
            {
                return _apiService.GetAsync<PricingResult>("OperationsAPI", $"api/pricing/final-price/{profileId}/{basePrice}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PricingDto> CreatePricingAsync(PricingDto pricing)
        {
            try
            {
                var response = await _apiService.PostAsync<PricingDto, PricingDto>("OperationsAPI", "api/pricing", pricing).ConfigureAwait(false);
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

        public async Task<PricingDto> UpdatePricingAsync(PricingDto pricing)
        {
            try
            {
                var response = await _apiService.PutAsync<PricingDto, PricingDto>("OperationsAPI", $"api/pricing/{pricing.PricingId}", pricing).ConfigureAwait(false);
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

        public async Task<PricingDto> DeletePricingAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<PricingDto>("OperationsAPI", $"api/pricing/{id}").ConfigureAwait(false);
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
