using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class RolePermissionDto
    {
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public bool RoleIsActive { get; set; }

        public Guid ModuleId { get; set; }

        public string ModuleName { get; set; }

        public Guid PermissionId { get; set; }

        public string PermissionName { get; set; }

        public string Access => $"{ModuleName}.{PermissionName}";
    }
}
