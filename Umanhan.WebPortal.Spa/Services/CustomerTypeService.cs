using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class CustomerTypeService
    {
        private readonly ApiService _apiService;

        public CustomerTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<CustomerTypeDto>>> GetAllCustomerTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<CustomerTypeDto>>("MasterdataAPI", "api/customer-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<CustomerTypeDto>> GetCustomerTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<CustomerTypeDto>("MasterdataAPI", $"api/customer-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomerTypeDto> CreateCustomerTypeAsync(CustomerTypeDto customerType)
        {
            try
            {
                var response = await _apiService.PostAsync<CustomerTypeDto, CustomerTypeDto>("MasterdataAPI", "api/customer-types", customerType).ConfigureAwait(false);
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

        public async Task<CustomerTypeDto> UpdateCustomerTypeAsync(CustomerTypeDto customerType)
        {
            try
            {
                var response = await _apiService.PutAsync<CustomerTypeDto, CustomerTypeDto>("MasterdataAPI", $"api/customer-types/{customerType.TypeId}", customerType).ConfigureAwait(false);
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

        public async Task<CustomerTypeDto> DeleteCustomerTypeAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<CustomerTypeDto>("MasterdataAPI", $"api/customer-types/{id}").ConfigureAwait(false);
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
