using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class PricingProfileService
    {
        private readonly ApiService _apiService;

        public PricingProfileService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<PricingProfileDto>> GetPricingProfileByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<PricingProfileDto>("OperationsAPI", $"api/pricing-profiles/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<PricingProfileDto>>> GetPricingProfilesByFarmIdAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<PricingProfileDto>>("OperationsAPI", $"api/pricing-profiles/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PricingProfileDto> CreatePricingProfileAsync(PricingProfileDto profile)
        {
            try
            {
                var response = await _apiService.PostAsync<PricingProfileDto, PricingProfileDto>("OperationsAPI", "api/pricing-profiles", profile).ConfigureAwait(false);
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

        public async Task<PricingProfileDto> UpdatePricingProfileAsync(PricingProfileDto profile)
        {
            try
            {
                var response = await _apiService.PutAsync<PricingProfileDto, PricingProfileDto>("OperationsAPI", $"api/pricing-profiles/{profile.ProfileId}", profile).ConfigureAwait(false);
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

        public async Task<PricingProfileDto> DeletePricingProfileAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<PricingProfileDto>("OperationsAPI", $"api/pricing-profiles/{id}").ConfigureAwait(false);
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
