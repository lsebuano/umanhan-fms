using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmActivity : IEntity
{
    [Column("ActivityId")]
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    /// <summary>
    /// id from either crops or livestocks tables, no FK
    /// </summary>
    public Guid ProductId { get; set; }

    public Guid FarmId { get; set; }

    public Guid SupervisorId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; }

    public string Notes { get; set; }

    public Guid ProductTypeId { get; set; }

    public Guid? ContractId { get; set; }

    public virtual FarmContract Contract { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ProductType ProductType { get; set; }

    public virtual Staff Supervisor { get; set; }

    public virtual Task Task { get; set; }

    public virtual ICollection<FarmActivityExpense> FarmActivityExpenses { get; set; } = [];

    public virtual ICollection<FarmActivityLaborer> FarmActivityLaborers { get; set; } = [];

    public virtual ICollection<FarmActivityUsage> FarmActivityUsages { get; set; } = [];

    public virtual ICollection<FarmActivityPhoto> FarmActivityPhotos { get; set; } = [];
}
