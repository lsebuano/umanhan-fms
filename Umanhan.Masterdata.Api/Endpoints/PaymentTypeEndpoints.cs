using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class PaymentTypeEndpoints
    {
        private readonly PaymentTypeService _paymentTypeService;
        private readonly IValidator<PaymentTypeDto> _validator;
        private readonly ILogger<PaymentTypeEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "paymenttypes";

        public PaymentTypeEndpoints(PaymentTypeService paymentTypeService, IValidator<PaymentTypeDto> validator, ILogger<PaymentTypeEndpoints> logger)
        {
            _paymentTypeService = paymentTypeService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllPaymentTypesAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _paymentTypeService.GetAllPaymentTypesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment types");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPaymentTypeByIdAsync(Guid id)
        {
            try
            {
                var paymentType = await _paymentTypeService.GetPaymentTypeByIdAsync(id).ConfigureAwait(false);
                return paymentType is not null ? Results.Ok(paymentType) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment type with ID {PaymentTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreatePaymentTypeAsync(PaymentTypeDto paymentType)
        {
            var validationResult = await _validator.ValidateAsync(paymentType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newPaymentType = await _paymentTypeService.CreatePaymentTypeAsync(paymentType).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/payment-types/{newPaymentType.PaymentTypeId}", newPaymentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment type");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdatePaymentTypeAsync(Guid id, PaymentTypeDto paymentType)
        {
            if (id != paymentType.PaymentTypeId)
                return Results.BadRequest("Payment Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(paymentType).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedPaymentType = await _paymentTypeService.UpdatePaymentTypeAsync(paymentType).ConfigureAwait(false);
                if (updatedPaymentType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedPaymentType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment type with ID {PaymentTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeletePaymentTypeAsync(Guid id)
        {
            try
            {
                var deletedPaymentType = await _paymentTypeService.DeletePaymentTypeAsync(id).ConfigureAwait(false);
                if (deletedPaymentType is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedPaymentType);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment type with ID {PaymentTypeId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
