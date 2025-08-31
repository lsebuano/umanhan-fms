using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Umanhan.Models.Dtos
{
    public class FarmActivityLaborerDto
    {
        public Guid LaborActivityId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid LaborerId { get; set; }

        public Guid PaymentTypeId { get; set; }

        public decimal Rate { get; set; }

        public short QuantityWorked { get; set; }

        public string PaymentType { get; set; }

        public string LaborName { get; set; }

        public decimal TotalPayment { get; set; }

        public DateTime Timestamp { get; set; }

        public void Recompute()
        {
            if (string.Equals(PaymentType, "CONTRACT", StringComparison.OrdinalIgnoreCase))
            {
                TotalPayment = Rate;
            }
            else
            {
                TotalPayment = Rate * QuantityWorked;
            }
        }
    }
}
