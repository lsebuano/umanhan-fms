using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class PermissionRepository : UmanhanRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
