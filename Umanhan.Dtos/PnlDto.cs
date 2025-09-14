namespace Umanhan.Dtos
{
    public class PnlDto : BaseDto
    {
        public DateTime Period { get; set; }    // use month/year, or quarterly
        public decimal Revenue { get; set; }
        public decimal CostOfGoodsSold { get; set; }
        public decimal OperatingExpenses { get; set; }
        public decimal NetProfit => Revenue - CostOfGoodsSold - OperatingExpenses;
    }
}
