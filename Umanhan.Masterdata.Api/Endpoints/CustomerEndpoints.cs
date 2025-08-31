using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class CustomerEndpoints
    {
        private readonly CustomerService _customerService;
        private readonly IValidator<CustomerDto> _validator;
        private readonly ILogger<CustomerEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "customer";

        public CustomerEndpoints(CustomerService customerService, 
            IValidator<CustomerDto> validator, 
            ILogger<CustomerEndpoints> logger)
        {
            _customerService = customerService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllCustomersAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _customerService.GetAllCustomersAsync("CustomerType", "FarmContracts.Farm").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetCustomersContractEligibleAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list:elligible";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _customerService.GetCustomersContractEligibleAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract eligible customers");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetCustomersByTypeAsync(Guid customerTypeId)
        {
            try
            {
                var customers = await _customerService.GetCustomersByTypeAsync(customerTypeId, "CustomerType", "FarmContracts.Farm").ConfigureAwait(false);
                return Results.Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers by type.");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetCustomerByIdAsync(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id, "CustomerType", "FarmContracts.Farm").ConfigureAwait(false);
                return customer is not null ? Results.Ok(customer) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateCustomerAsync(CustomerDto customer)
        {
            var validationResult = await _validator.ValidateAsync(customer).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newCustomer = await _customerService.CreateCustomerAsync(customer).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                //key = $"{MODULE_CACHE_KEY}:list:elligible";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/customers/{newCustomer.CustomerId}", newCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateCustomerAsync(Guid id, CustomerDto customer)
        {
            if (id != customer.CustomerId)
                return Results.BadRequest("Customer ID mismatch");

            var validationResult = await _validator.ValidateAsync(customer).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);
                if (updatedCustomer is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedCustomer);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID {CustomerId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteCustomerAsync(Guid id)
        {
            try
            {
                var deletedCustomer = await _customerService.DeleteCustomerAsync(id).ConfigureAwait(false);
                if (deletedCustomer is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedCustomer);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID {CustomerId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
