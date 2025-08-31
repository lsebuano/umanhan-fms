using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class CustomerService
    {
        private readonly ApiService _apiService;

        public CustomerService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<CustomerDto>>> GetAllCustomersAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<CustomerDto>>("MasterdataAPI", "api/customers");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<CustomerDto>>> GetCustomersContractEligibleAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<CustomerDto>>("MasterdataAPI", "api/customers/contract-eligible");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<CustomerDto>("MasterdataAPI", $"api/customers/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customer)
        {
            try
            {
                var response = await _apiService.PostAsync<CustomerDto, CustomerDto>("MasterdataAPI", "api/customers", customer).ConfigureAwait(false);
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

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customer)
        {
            try
            {
                var response = await _apiService.PutAsync<CustomerDto, CustomerDto>("MasterdataAPI", $"api/customers/{customer.CustomerId}", customer).ConfigureAwait(false);
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

        public async Task<CustomerDto> DeleteCustomerAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<CustomerDto>("MasterdataAPI", $"api/customers/{id}").ConfigureAwait(false);
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
