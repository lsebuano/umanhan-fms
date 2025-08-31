using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Dtos
{
    public class SparklineChartDto
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
