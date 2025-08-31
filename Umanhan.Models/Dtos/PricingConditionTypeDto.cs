using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class PricingConditionTypeDto
    {
        public Guid ConditionId { get; set; }

        public string Name { get; set; }

        public bool IsDeduction { get; set; }
    }
}
