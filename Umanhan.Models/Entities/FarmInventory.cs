using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

/// <summary>
/// For farm equipments, materials, tools, etc
/// </summary>
public partial class FarmInventory : IEntity
{
    [Column("FarmInventoryId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid InventoryId { get; set; }

    public Guid UnitId { get; set; }

    public decimal Quantity { get; set; }

    public string Status { get; set; }

    public string Notes { get; set; }

    public string ItemImageS3KeyThumbnail { get; set; }

    public string ItemImageS3ContentType { get; set; }

    public string ItemImageS3KeyFull { get; set; }

    public string ItemImageS3UrlThumbnail { get; set; }

    public string ItemImageS3UrlFull { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual Inventory Inventory { get; set; }

    public virtual Unit Unit { get; set; }
}
