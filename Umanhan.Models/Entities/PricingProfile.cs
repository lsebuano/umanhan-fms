using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class PricingProfile : IEntity
{
    [Column("ProfileId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal FinalPrice { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ICollection<PricingCondition> PricingConditions { get; set; } = [];

    public virtual ICollection<FarmContractDetail> FarmContractDetails { get; set; } = [];

    public virtual ICollection<Quotation> Quotations { get; set; } = [];
}
