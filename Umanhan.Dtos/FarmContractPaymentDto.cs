using Umanhan.Shared.Utils;

namespace Umanhan.Dtos
{
    public class FarmContractPaymentDto
    {
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public string FarmFullAddress { get; set; }
        public string FarmTelephone { get; set; }
        public string FarmEmail { get; set; }
        public string FarmTin { get; set; }

        public string ReceiptNo { get; set; }
        public string Date { get; set; }

        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerTin { get; set; }
        public string BuyerContactNo { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentRef { get; set; }
        public string AmountInWords => NumberToWordsConverter.ConvertAmountToPesos(TotalAmount);

        public string ReceivedBy { get; set; }
        public string PrintedBy { get; set; }
        public string PrintTimestamp { get; set; }

        public decimal Subtotal { get; set; }
        public decimal TotalAdjustments { get; set; }
        public decimal TotalAmount { get; set; }

        public List<FarmContractPaymentDetailsDto> Items { get; set; } = new List<FarmContractPaymentDetailsDto>();
    }
}
