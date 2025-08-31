using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class SoilType : IEntity
{
    [Column("SoilId")]
    public Guid Id { get; set; }

    public string SoilName { get; set; }

    /// <summary>
    /// Acidic, Neutral, Alkaline
    /// </summary>
    public string SoilPh { get; set; }

    /// <summary>
    /// Low, Medium, High
    /// </summary>
    public string SoilOrganicCarbon { get; set; }

    /// <summary>
    /// Dry, Moist, Wet
    /// </summary>
    public string SoilMoisture { get; set; }

    /// <summary>
    /// Low, Medium, High
    /// </summary>
    public string SoilFertility { get; set; }

    public string Notes { get; set; }

    public virtual ICollection<FarmZone> FarmZones { get; set; } = [];
}
