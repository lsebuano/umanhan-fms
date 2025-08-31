using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Livestock : IEntity
{
    [Column("LivestockId")]
    public Guid Id { get; set; }

    public string AnimalType { get; set; }

    public string Breed { get; set; }

    public virtual ICollection<FarmLivestock> FarmLivestocks { get; set; } = [];
}
