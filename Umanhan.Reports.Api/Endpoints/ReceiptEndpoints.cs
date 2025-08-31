using Umanhan.Models.Dtos;
using Umanhan.Services;

namespace Umanhan.Reports.Api.Endpoints
{
    public class ReceiptEndpoints
    {
        private readonly ContractPaymentService _contractPaymentService;
        private readonly ILogger<ReceiptEndpoints> _logger;

        public ReceiptEndpoints(ContractPaymentService contractPaymentService, ILogger<ReceiptEndpoints> logger)
        {
            _contractPaymentService = contractPaymentService;
            _logger = logger;
        }

        public async Task<IResult> GetReceiptByIdAsync(string paymentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId))
                {
                    _logger.LogWarning("Payment ID is null or empty.");
                    return Results.BadRequest("Payment ID cannot be null or empty.");
                }

                var payment = await _contractPaymentService.GetAsync(paymentId).ConfigureAwait(false);
                return Results.Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving receipt with ID {PaymentId}", paymentId);
                return Results.Problem("An error occurred while retrieving the receipt.");
            }
        }

        public async Task<IResult> GenerateReceiptAsync(PaymentDetailsDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Received null FarmContractPaymentDto.");
                    return Results.BadRequest("FarmContractPaymentDto cannot be null.");
                }

                var receipt = await _contractPaymentService.GenerateReceiptAsync(dto).ConfigureAwait(false);
                return Results.Ok(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating receipt.");
                return Results.Problem("An error occurred while generating the receipt.");
            }
        }
    }
}
