using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmActivityExpense : IEntity
{
    [Column("ExpenseId")]
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }

    public Guid ExpenseTypeId { get; set; }

    public string Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public virtual FarmActivity Activity { get; set; }

    public virtual ExpenseType ExpenseType { get; set; }
}
