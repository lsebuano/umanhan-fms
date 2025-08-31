using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmZone : IEntity
{
    [Column("ZoneId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid? SoilId { get; set; }

    public string ZoneName { get; set; }

    public string ZoneDescription { get; set; }

    public decimal? SizeInHectares { get; set; }

    public decimal? SizeInSqm { get; set; }

    public string IrrigationType { get; set; }

    public string BoundaryJson { get; set; }

    public string ZoneColor { get; set; }

    public double? Lat { get; set; }

    public double? Lng { get; set; }

    public string Notes { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ICollection<FarmCrop> FarmCrops { get; set; } = [];

    public virtual ICollection<FarmLivestock> FarmLivestocks { get; set; } = [];

    public virtual ICollection<FarmZoneYield> FarmZoneYields { get; set; } = [];

    public virtual SoilType Soil { get; set; }
}
