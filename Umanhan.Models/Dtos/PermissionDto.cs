using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class PermissionDto
    {
        public Guid PermissionId { get; set; }

        public string Name { get; set; }

        public IEnumerable<RolePermission> RolePermissions { get; set; } = [];
    }
}
