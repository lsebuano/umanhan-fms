using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmContract : IEntity
{
    [Column("ContractId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid CustomerId { get; set; }

    public DateOnly ContractDate { get; set; }

    /// <summary>
    /// Pending, Completed, Cancelled
    /// </summary>
    public string Status { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = [];

    public virtual ICollection<FarmContractDetail> FarmContractDetails { get; set; } = [];
}
