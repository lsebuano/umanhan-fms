using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class PricingProfileDto
    {
        public Guid ProfileId { get; set; }

        public Guid FarmId { get; set; }

        public string FarmName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal FinalPrice { get; set; }

        public IEnumerable<PricingDto> PricingConditions { get; set; } = [];
    }
}
