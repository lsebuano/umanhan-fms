using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmLivestock : IEntity
{
    [Column("FarmLivestockId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid LivestockId { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Breed { get; set; }

    public Guid ZoneId { get; set; }

    public int Quantity { get; set; }

    public decimal PurchaseCost { get; set; }

    public Guid UnitId { get; set; }

    public decimal DefaultRate { get; set; }

    public DateOnly? EstimatedHarvestDate { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual Livestock Livestock { get; set; }

    public virtual FarmZone Zone { get; set; }
}
