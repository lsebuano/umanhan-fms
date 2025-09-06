namespace Umanhan.Dtos.HelperModels
{
    public class PricingResult
    {
        public decimal BasePrice { get; set; }
        public List<PricingDto> Breakdown { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
