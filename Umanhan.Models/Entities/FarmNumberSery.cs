using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmNumberSery : IEntity
{
    [Column("NumberSeryId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public string Type { get; set; }

    public int CurrentSery { get; set; }

    public string Format { get; set; }

    public virtual Farm Farm { get; set; }
}
