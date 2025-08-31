using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class PaymentTypeDto
    {
        public Guid PaymentTypeId { get; set; }

        public string PaymentTypeName { get; set; }

        public IEnumerable<FarmActivityLaborerDto> FarmActivityLaborers { get; set; } = [];
    }
}
