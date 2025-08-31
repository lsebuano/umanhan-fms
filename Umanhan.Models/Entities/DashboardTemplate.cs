using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class DashboardTemplate : IEntity
{
    [Column("TemplateId")]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string DashboardComponentName { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = [];
}
