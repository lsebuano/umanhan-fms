namespace Umanhan.Dtos
{
    public class MonthlySalesDto : BaseDto
    {
        public DateTime Month { get; set; }
        public int MonthNumber => Month.Month;
        public string MonthName => Month.ToString("MMM");
        public string MonthNameFull => Month.ToString("MMMM");
        public decimal TotalAmount { get; set; }
        public string Customer { get; set; }
    }
}
