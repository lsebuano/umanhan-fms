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
    public class FarmActivityExpenseDto
    {
        public Guid ExpenseId { get; set; }

        public Guid ActivityId { get; set; }

        public string Task { get; set; }

        public Guid ExpenseTypeId { get; set; }

        public string ExpenseTypeName { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string ProductType { get; set; }

        public string Supervisor { get; set; }

        public DateTime ActivityStartDateTime { get; set; }

        public DateTime ActivityEndDateTime { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerContactInfo { get; set; }

        public string FarmName { get; set; }

        public string FarmLocation { get; set; }

        public DateOnly DateUtc { get; set; }

        public DateOnly Date => DateUtc.ToManilaTime();
    }
}
