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
    public class RoleDto
    {
        public Guid RoleId { get; set; }

        public string GroupName { get; set; }

        public bool IsActive { get; set; }

        public Guid? TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string DashboardTemplateComponentName { get; set; }

        public IEnumerable<RolePermissionDto> RolePermissions { get; set; } = [];
    }
}
