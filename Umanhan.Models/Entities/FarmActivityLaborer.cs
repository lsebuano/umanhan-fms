using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmActivityLaborer : IEntity
{
    [Column("LaborActivityId")]
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }

    public Guid LaborerId { get; set; }

    public Guid PaymentTypeId { get; set; }

    public decimal Rate { get; set; }

    public short QuantityWorked { get; set; }

    public decimal TotalPayment { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual FarmActivity Activity { get; set; }

    public virtual Laborer Laborer { get; set; }

    public virtual PaymentType PaymentType { get; set; }
}
