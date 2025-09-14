using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmGeneralExpense : IEntity
{
    [Column("ExpenseId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid ExpenseTypeId { get; set; }

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string Payee { get; set; }

    public string Notes { get; set; }

    public virtual ExpenseType ExpenseType { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ICollection<FarmGeneralExpenseReceipt> FarmGeneralExpenseReceipts { get; set; } = [];
}
