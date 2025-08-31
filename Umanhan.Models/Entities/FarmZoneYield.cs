using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmZoneYield : IEntity
{
    [Column("YieldId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid ZoneId { get; set; }

    public Guid ProductTypeId { get; set; }

    public Guid ProductId { get; set; }

    public Guid UnitId { get; set; }

    public Guid ContractDetailId { get; set; }

    public decimal ExpectedYield { get; set; }

    public decimal? ActualYield { get; set; }

    public decimal? ForecastedYield { get; set; }

    public DateOnly HarvestDate { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ProductType ProductType { get; set; }

    public virtual FarmZone Zone { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual FarmContractDetail FarmContractDetail { get; set; }
}
