using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class ProductType : IEntity
{
    [Column("TypeId")]
    public Guid Id { get; set; }

    public string ProductTypeName { get; set; }

    public string TableName { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = [];

    public virtual ICollection<FarmContractDetail> FarmContractDetails { get; set; } = [];

    public virtual ICollection<FarmProduceInventory> FarmProduceInventories { get; set; } = [];

    public virtual ICollection<FarmTransaction> FarmTransactions { get; set; } = [];

    public virtual ICollection<QuotationProduct> QuotationProducts { get; set; } = [];

    public virtual ICollection<FarmZoneYield> FarmZoneYields { get; set; } = [];
}
