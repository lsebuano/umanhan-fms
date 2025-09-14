namespace Umanhan.Dtos
{
    public class FarmContractPaymentDetailsDto : BaseDto
    {
        public string Item { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string Unit { get; set; }
    }
}
