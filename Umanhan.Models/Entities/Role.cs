using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public class Role : IEntity
    {
        [Column("RoleId")]
        public Guid Id { get; set; }

        public Guid? TemplateId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public virtual DashboardTemplate DashboardTemplate { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
