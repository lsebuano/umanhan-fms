using FluentValidation;
using Umanhan.Masterdata.Api;
using Umanhan.Models.Dtos;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class CustomerTypeEndpoints
    {
        private readonly CustomerTypeService _customerTypeService;
        private readonly IValidator<CustomerTypeDto> _validator;
        private readonly ILogger<CustomerTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "customertype";

        public CustomerTypeEndpoints(CustomerTypeService customerTypeService, IValidator<CustomerTypeDto> validator, ILogger<CustomerTypeEndpoints> logger)
        {
            _customerTypeService = customerTypeService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllCustomerTypesAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _customerTypeService.GetAllCustomerTypesAsync("Customers").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetCustomerTypeByIdAsync(Guid id)
        {
            try
            {
                var customerType = await _customerTypeService.GetCustomerTypeByIdAsync(id, "Customers").ConfigureAwait(false);
                return customerType is not null ? Results.Ok(customerType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer type with ID {CustomerTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateCustomerTypeAsync(CustomerTypeDto customerType)
        {
            var validationResult = await _validator.ValidateAsync(customerType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newCustomerType = await _customerTypeService.CreateCustomerTypeAsync(customerType).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/customer-types/{newCustomerType.TypeId}", newCustomerType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer type");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateCustomerTypeAsync(Guid id, CustomerTypeDto customerType)
        {
            if (id != customerType.TypeId)
                return Results.BadRequest("Customer Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(customerType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedCustomerType = await _customerTypeService.UpdateCustomerTypeAsync(customerType).ConfigureAwait(false);
                if (updatedCustomerType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedCustomerType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer type with ID {CustomerTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteCustomerTypeAsync(Guid id)
        {
            try
            {
                var deletedCustomerType = await _customerTypeService.DeleteCustomerTypeAsync(id).ConfigureAwait(false);
                if (deletedCustomerType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedCustomerType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer type with ID {CustomerTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
