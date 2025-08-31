using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Task : IEntity
{
    [Column("TaskId")]
    public Guid Id { get; set; }

    public string TaskName { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual Category Category { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = [];
}
