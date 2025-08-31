using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class PricingConditionType : IEntity
{
    [Column("ConditionId")]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsDeduction { get; set; }

    public virtual ICollection<PricingCondition> PricingConditions { get; set; } = [];
}
