using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public class Module : IEntity
    {
        [Column("ModuleId")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
