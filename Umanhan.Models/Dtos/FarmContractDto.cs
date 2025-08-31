using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class FarmContractDto
    {
        public Guid ContractId { get; set; }

        public Guid FarmId { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime ContractDate { get; set; }

        public string Status { get; set; }

        public string CustomerName { get; set; }

        public string CustomerTin { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerContactInfo { get; set; }

        public string FarmName { get; set; }

        public string FarmLocation { get; set; }

        public string FarmFullAddress { get; set; }

        public bool IsCancelled => Status.ToUpper() == ContractStatus.CANCELLED.ToString();

        public bool IsPaid => Status.ToUpper() == ContractStatus.PAID.ToString() || Status.ToUpper() == ContractStatus.PARTIALLY_PAID.ToString();

        public IEnumerable<FarmContractDetailDto> FarmContractDetails { get; set; } = [];
    }
}
