using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class PaymentType : IEntity
{
    [Column("PaymentTypeId")]
    public Guid Id { get; set; }

    public string PaymentTypeName { get; set; }

    public virtual ICollection<FarmActivityLaborer> FarmActivityLaborers { get; set; } = [];
}
