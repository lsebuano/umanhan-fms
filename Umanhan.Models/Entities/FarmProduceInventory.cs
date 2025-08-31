using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

/// <summary>
/// For long-storage farm produce. Entries here are also for sale or for surplus donation.
/// </summary>
public partial class FarmProduceInventory : IEntity
{
    [Column("InventoryId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    /// <summary>
    /// Either crop_id or livestock_id. no FK
    /// </summary>
    public Guid ProductId { get; set; }

    public Guid ProductTypeId { get; set; }

    public decimal InitialQuantity { get; set; }

    public Guid UnitId { get; set; }

    public DateOnly Date { get; set; }

    public decimal UnitPrice { get; set; }

    public string Notes { get; set; }

    public decimal CurrentQuantity { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual ProductType ProductType { get; set; }

    public virtual ICollection<FarmTransaction> FarmTransactions { get; set; } = [];
}
