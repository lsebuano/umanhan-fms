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
    public class FarmGeneralExpenseDto
    {
        public Guid ExpenseId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ExpenseTypeId { get; set; }

        public DateOnly Date { get; set; }

        public decimal Amount { get; set; }

        public string Payee { get; set; }

        public string Notes { get; set; }

        public string ExpenseTypeName { get; set; }

        public string FarmName { get; set; }

        public Guid RowKey => Guid.NewGuid();
    }
}
