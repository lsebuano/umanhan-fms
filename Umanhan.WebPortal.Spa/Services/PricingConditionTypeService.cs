using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class PricingConditionTypeService
    {
        private readonly ApiService _apiService;

        public PricingConditionTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<PricingConditionTypeDto>>> GetPricingConditionTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<PricingConditionTypeDto>>("MasterdataAPI", "api/condition-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<PricingConditionTypeDto>> GetPricingConditionTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<PricingConditionTypeDto>("MasterdataAPI", $"api/condition-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
