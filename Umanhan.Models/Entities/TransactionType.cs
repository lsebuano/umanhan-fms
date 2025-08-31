using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class TransactionType : IEntity
{
    [Column("TypeId")]
    public Guid Id { get; set; }

    public string TransactionTypeName { get; set; }

    public virtual ICollection<FarmTransaction> FarmTransactions { get; set; } = [];
}
