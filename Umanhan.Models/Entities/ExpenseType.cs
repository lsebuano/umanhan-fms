using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class ExpenseType : IEntity
{
    [Column("TypeId")]
    public Guid Id { get; set; }

    public string ExpenseTypeName { get; set; }

    public virtual ICollection<FarmActivityExpense> FarmActivityExpenses { get; set; } = [];
    public virtual ICollection<FarmGeneralExpense> FarmGeneralExpenses { get; set; } = [];
}
