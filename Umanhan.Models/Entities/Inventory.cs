using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Inventory : IEntity
{
    [Column("InventoryId")]
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? UnitId { get; set; }

    public string ItemName { get; set; }

    public virtual Category Category { get; set; }

    public virtual FarmInventory FarmInventory { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual ICollection<FarmActivityUsage> FarmActivityUsages { get; set; } = [];
}
