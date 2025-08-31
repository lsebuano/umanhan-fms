using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Dtos
{
    public class MonthlySalesDto
    {
        public DateTime Month { get; set; }
        public int MonthNumber => Month.Month;
        public string MonthName => Month.ToString("MMM");
        public string MonthNameFull => Month.ToString("MMMM");
        public decimal TotalAmount { get; set; }
        public string Customer { get; set; }
    }
}
