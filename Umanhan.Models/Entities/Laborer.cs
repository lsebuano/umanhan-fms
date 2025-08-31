using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Laborer : IEntity
{
    [Column("LaborerId")]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Skillset { get; set; }

    public string ContactInfo { get; set; }

    public decimal? DailyRate { get; set; }

    public decimal? ContractedRate { get; set; }

    public virtual ICollection<FarmActivityLaborer> FarmActivityLaborers { get; set; } = [];
}
