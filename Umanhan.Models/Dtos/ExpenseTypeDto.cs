using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class ExpenseTypeDto
    {
        public Guid TypeId { get; set; }

        public string ExpenseTypeName { get; set; }

        public FarmActivityExpenseDto FarmActivityExpense { get; set; }
    }
}
