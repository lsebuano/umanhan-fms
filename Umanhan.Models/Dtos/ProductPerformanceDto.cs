using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Dtos
{
    public class ProductPerformanceDto
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
