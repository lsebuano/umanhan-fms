using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string Address { get; set; }

        public string ContactInfo { get; set; }

        public Guid CustomerTypeId { get; set; }

        public string CustomerType { get; set; }

        public string EmailAddress { get; set; }

        public bool ContractEligible { get; set; }

        public string ContractEligibleString => ContractEligible ? "Yes" : "No";

        public IEnumerable<FarmContractDto> FarmContracts { get; set; } = [];

        public IEnumerable<FarmTransactionDto> FarmTransactions { get; set; } = [];
    }
}
