namespace Umanhan.Dtos
{
    public class PaymentDetailsDto : BaseDto
    {
        public Guid ContractId { get; set; }

        public Guid? BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerTin { get; set; }
        public string BuyerContactNo { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentRef { get; set; }
        public decimal ActualPaidAmount { get; set; }

        public string ReceivedBy { get; set; }
        public string SystemRefNo { get; set; }
    }
}
