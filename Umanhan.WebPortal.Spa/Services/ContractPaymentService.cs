using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ContractPaymentService
    {
        private readonly ApiService _apiService;

        public ContractPaymentService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<FarmContractPaymentDto>> GetPaymentByIdAsync(string paymentId)
        {
            try
            {
                return _apiService.GetAsync<FarmContractPaymentDto>("ReportAPI", $"api/receipts/{paymentId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
