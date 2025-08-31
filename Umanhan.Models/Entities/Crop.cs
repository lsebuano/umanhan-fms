using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Crop : IEntity
{
    [Column("CropId")]
    public Guid Id { get; set; }

    public string CropName { get; set; }

    public string CropVariety { get; set; }

    public string Notes { get; set; }

    public decimal? DefaultRatePerUnit { get; set; }

    public Guid DefaultUnitId { get; set; }

    public virtual Unit DefaultUnit { get; set; }

    public virtual ICollection<FarmCrop> FarmCrops { get; set; } = [];
}
