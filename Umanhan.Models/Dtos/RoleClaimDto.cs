using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Dtos
{
    public class RoleClaimDto
    {
        public IEnumerable<RolePermissionDto> Permissions { get; set; } = [];
        public string DashboardComponent { get; set; }
    }
}
