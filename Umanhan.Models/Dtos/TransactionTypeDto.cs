using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class TransactionTypeDto
    {
        public Guid TypeId { get; set; }

        public string TransactionTypeName { get; set; }

        public IEnumerable<FarmTransactionDto> FarmTransactions { get; set; } = [];
    }
}
