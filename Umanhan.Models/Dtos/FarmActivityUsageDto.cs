using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class FarmActivityUsageDto
    {
        public Guid UsageId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid InventoryId { get; set; }

        public Guid? UnitId { get; set; }

        public string ItemName { get; set; }

        public decimal UsageHours { get; set; }

        public decimal Rate { get; set; }

        public string Unit { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime Timestamp { get; set; }

        public void Recompute()
        {
            TotalCost = Rate * UsageHours;
        }
    }
}
