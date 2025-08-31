using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmCrop : IEntity
{
    [Column("FarmCropId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid CropId { get; set; }

    public Guid UnitId { get; set; }

    public Guid ZoneId { get; set; }

    public decimal DefaultRate { get; set; }

    public DateOnly? PlantingDate { get; set; }

    public DateOnly? EstimatedHarvestDate { get; set; }

    public virtual Crop Crop { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual FarmZone Zone { get; set; }
}
