using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;
using Umanhan.Shared;
using Umanhan.Shared.Utils;

namespace Umanhan.Services
{
    public class ContractPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<ContractPaymentService> _logger;

        public ContractPaymentService(IUnitOfWork unitOfWork, IUserContextService userContext, ILogger<ContractPaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
        }

        private static FarmContractPaymentDto ToFarmContractPaymentDto(FarmContractPayment obj)
        {
            return new FarmContractPaymentDto
            {
                FarmId = obj.FarmId,
                FarmEmail = obj.FarmContactEmail,
                FarmTelephone = obj.FarmContactPhone,
                BuyerContactNo = obj.BuyerContactNo,
                FarmFullAddress = obj.FarmAddress,
                Subtotal = obj.Subtotal,
                ReceiptNo = obj.InvoiceNo,
                BuyerName = obj.BuyerName,
                BuyerTin = obj.BuyerTin,
                Date = obj.Date.ToString("MM-dd-yyyy"),
                FarmName = obj.FarmName,
                FarmTin = obj.FarmTin,
                PaymentMethod = obj.PaymentMethod,
                PaymentRef = obj.PaymentRef,
                PrintedBy = obj.PrintedBy,
                PrintTimestamp = obj.PrintTimestamp?.ToString("MM-dd-yyyy HH:mm:ss"),
                ReceivedBy = obj.ReceivedBy,
                TotalAdjustments = obj.TotalAdjustments,
                TotalAmount = obj.TotalAmountReceived,
                Items = [.. obj.FarmContractPaymentDetails.Select(item => new FarmContractPaymentDetailsDto
                {
                    Item = item.Item,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.TotalAmount,
                    Unit = item.Unit,
                })]
            };
        }

        public async Task<FarmContractPaymentDto> GenerateReceiptAsync(PaymentDetailsDto dto)
        {
            try
            {
                var contractObj = await _unitOfWork.FarmContracts.GetByIdAsync(dto.ContractId, "Farm", "Customer", "FarmContractDetails.Unit").ConfigureAwait(false);
                if (contractObj == null)
                {
                    _logger.LogWarning("Contract with ID {ContractId} not found.", dto.ContractId);
                    throw new KeyNotFoundException($"Contract with ID {dto.ContractId} not found.");
                }

                var totalAmount = contractObj.FarmContractDetails
                    .Where(x => x.Status.Equals(ContractStatus.PAID.ToString()))
                    .Sum(x => x.TotalAmount);
                var actualPaidAmount = dto.ActualPaidAmount;
                var totalAdjustments = 0m;

                string documentType = "RECEIPT";
                var productLookup = await _unitOfWork.ProductLookup.BuildProductLookupAsync().ConfigureAwait(false);
                string invoiceNo = await _unitOfWork.FarmNumberSeries.GenerateNumberSeryAsync(contractObj.FarmId, documentType).ConfigureAwait(false);
                string systemRefNo = string.Format(dto.SystemRefNo, invoiceNo);
                // generate a payment record
                var paymentEntity = new FarmContractPayment
                {
                    DocumentType = documentType,
                    InvoiceNo = invoiceNo,
                    SystemRefNo = systemRefNo,
                    ReceivedBy = dto.ReceivedBy,
                    AmountInWords = NumberToWordsConverter.ConvertAmountToPesos(actualPaidAmount),
                    Date = DateOnly.FromDateTime(DateTime.Now.ToLocalTime()),
                    PrintedBy = _userContext.Username,
                    PrintTimestamp = DateTime.SpecifyKind(DateTime.Now.ToLocalTime(), DateTimeKind.Utc),

                    //FarmLogo = contractObj.Farm.Logo,
                    FarmId = contractObj.FarmId,
                    FarmName = contractObj.Farm.FarmName,
                    FarmTin = contractObj.Farm.Tin ?? "-",
                    FarmContactEmail = contractObj.Farm.ContactEmail ?? "-",
                    FarmContactPhone = contractObj.Farm.ContactPhone ?? "-",
                    FarmAddress = contractObj.Farm.FullAddress ?? contractObj.Farm.Location ?? "-",

                    BuyerName = dto.BuyerName,
                    BuyerTin = dto.BuyerTin,
                    BuyerContactNo = dto.BuyerContactNo,
                    BuyerAddress = dto.BuyerAddress,

                    PaymentMethod = dto.PaymentMethod,
                    PaymentRef = dto.PaymentRef,

                    TotalAmountReceived = actualPaidAmount,
                    TotalAdjustments = totalAdjustments,
                    Subtotal = totalAmount - totalAdjustments,

                    FarmContractPaymentDetails = [.. contractObj.FarmContractDetails
                        .Where(x => x.Status.Equals(ContractStatus.PAID.ToString(), StringComparison.OrdinalIgnoreCase))
                        .Select(item => new FarmContractPaymentDetail
                        {
                            Item = $"{productLookup[new ProductKey(item.ProductId, item.ProductTypeId)].ProductName} {productLookup[new ProductKey(item.ProductId, item.ProductTypeId)].Variety}",
                            Quantity = item.ContractedQuantity,
                            UnitPrice = item.ContractedUnitPrice,
                            TotalAmount = item.TotalAmount,
                            Unit = item.Unit.UnitName,
                        })],
                };
                await _unitOfWork.FarmContractPayments.AddAsync(paymentEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);
                return ToFarmContractPaymentDto(paymentEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<FarmContractPaymentDto> GetAsync(string paymentId)
        {
            try
            {
                var obj = await _unitOfWork.FarmContractPayments.GetAsync(paymentId).ConfigureAwait(false);
                return ToFarmContractPaymentDto(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new();
            }
        }
    }
}
