using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;

namespace Umanhan.Models.Models
{
    public class PricingResult
    {
        public decimal BasePrice { get; set; }
        public List<PricingDto> Breakdown { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
