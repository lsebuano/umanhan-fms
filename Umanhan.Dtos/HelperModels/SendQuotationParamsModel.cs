namespace Umanhan.Dtos.HelperModels
{
    public class SendQuotationParamsModel
    {
        public string RfqNumber { get; set; }
        //public Guid PricingProfileId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public IEnumerable<QuotationProductDto> Products { get; set; } = [];
    }
}
