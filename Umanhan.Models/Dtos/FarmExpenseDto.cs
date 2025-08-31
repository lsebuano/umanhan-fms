using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class FarmExpenseDto
    {
        public Guid ExpenseId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ExpenseTypeId { get; set; }

        public string ReferenceType { get; set; }

        public Guid ReferenceId { get; set; }

        public Guid UnitId { get; set; }

        public string Task { get; set; }

        public string ExpenseTypeName { get; set; }

        public string FarmName { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string RecordedBy { get; set; }

        public DateOnly ExpenseDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
