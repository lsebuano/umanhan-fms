namespace Umanhan.Dtos
{
    public class SparklineChartDto : BaseDto
    {
        public DateOnly Date { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPrevious { get; set; }
        public int Month { get; set; }
        public string MonthString { get; set; }
        public string MonthYearString => $"{Date.Month.ToString("00")}/{Date.Year}";
        public decimal Difference => Total - TotalPrevious;
        public decimal PercentageChange => TotalPrevious <= 0 ? 0 : (Total - TotalPrevious) / TotalPrevious;
    }
}
