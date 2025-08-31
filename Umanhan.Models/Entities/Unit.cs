using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Unit : IEntity
{
    [Column("UnitId")]
    public Guid Id { get; set; }

    public string UnitName { get; set; }

    public virtual ICollection<Crop> Crops { get; set; } = new List<Crop>();

    public virtual ICollection<FarmContractDetail> FarmContractDetails { get; set; } = new List<FarmContractDetail>();

    public virtual ICollection<FarmCrop> FarmCrops { get; set; } = new List<FarmCrop>();

    public virtual ICollection<FarmInventory> FarmInventories { get; set; } = new List<FarmInventory>();

    public virtual ICollection<FarmTransaction> FarmTransactions { get; set; } = new List<FarmTransaction>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<FarmProduceInventory> FarmProduceInventories { get; set; } = new List<FarmProduceInventory>();

    public virtual ICollection<FarmActivityUsage> FarmActivityUsages { get; set; } = [];

    public virtual ICollection<QuotationProduct> QuotationProducts { get; set; } = [];

    public virtual ICollection<FarmZoneYield> FarmZoneYields { get; set; } = [];
}
