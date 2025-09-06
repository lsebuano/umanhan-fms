using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class PaymentTypeService
    {
        private readonly ApiService _apiService;

        public PaymentTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<PaymentTypeDto>>> GetAllPaymentTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<PaymentTypeDto>>("MasterdataAPI", "api/payment-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<PaymentTypeDto>> GetPaymentTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<PaymentTypeDto>("MasterdataAPI", $"api/payment-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaymentTypeDto> CreatePaymentTypeAsync(PaymentTypeDto paymentType)
        {
            try
            {
                var response = await _apiService.PostAsync<PaymentTypeDto, PaymentTypeDto>("MasterdataAPI", "api/payment-types", paymentType).ConfigureAwait(false);
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

        public async Task<PaymentTypeDto> UpdatePaymentTypeAsync(PaymentTypeDto paymentType)
        {
            try
            {
                var response = await _apiService.PutAsync<PaymentTypeDto, PaymentTypeDto>("MasterdataAPI", $"api/payment-types/{paymentType.PaymentTypeId}", paymentType).ConfigureAwait(false);
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

        public async Task<PaymentTypeDto> DeletePaymentTypeAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<PaymentTypeDto>("MasterdataAPI", $"api/payment-types/{id}").ConfigureAwait(false);
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
