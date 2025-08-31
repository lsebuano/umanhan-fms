using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Category : IEntity
{
    [Column("CategoryId")]
    public Guid Id { get; set; }

    public string CategoryName { get; set; }

    public string Group { get; set; }

    public string Group2 { get; set; }

    public string ConsumptionBehavior { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = [];

    public virtual ICollection<Task> Tasks { get; set; } = [];
}
