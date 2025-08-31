using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmActivityUsage : IEntity
{
    [Column("UsageId")]
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }

    public Guid InventoryId { get; set; }

    public Guid? UnitId { get; set; }

    public decimal UsageHours { get; set; }

    public decimal Rate { get; set; }

    public decimal TotalCost { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual FarmActivity Activity { get; set; }

    public virtual Inventory Inventory { get; set; }

    public virtual Unit Unit { get; set; }
}
