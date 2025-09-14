namespace Umanhan.Dtos
{
    public class ProductPerformanceDto : BaseDto
    {
        public string Product { get; set; }
        public string Variety { get; set; }
        public string Unit { get; set; }
        public decimal Yield { get; set; }
        public decimal Cogs { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal Sales { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}
