using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Staff : IEntity
{
    [Column("StaffId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public string Name { get; set; }

    public string ContactInfo { get; set; }

    public DateOnly? HireDate { get; set; }

    public string Status { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = [];
}
