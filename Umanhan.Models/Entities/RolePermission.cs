using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities
{
    public class RolePermission : IEntity
    {
        [Column("RolePermissionId")]
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Guid ModuleId { get; set; }

        public Guid PermissionId { get; set; }

        public virtual Role Role { get; set; }

        public virtual Module Module { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
