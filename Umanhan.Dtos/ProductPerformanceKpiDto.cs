namespace Umanhan.Dtos
{
    public class ProductPerformanceKpiDto : BaseDto
    {
        public string Product { get; set; }
        public string Variety { get; set; }
        public string Unit { get; set; }
        public decimal Value  { get; set; }
    }
}
