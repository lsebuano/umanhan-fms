using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class VwProductLookup : IEntity
{
    [NotMapped]
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? ProductTypeId { get; set; }

    public string ProductTypeName { get; set; }

    public string ProductName { get; set; }

    public string Variety { get; set; }

    public Guid? FarmId { get; set; }

    public decimal? DefaultRate { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EstimatedHarvestDate { get; set; }

    public string FarmName { get; set; }

    public Guid? UnitId { get; set; }

    public string UnitName { get; set; }

    public Guid? ZoneId { get; set; }

    public string ZoneName { get; set; }
}
