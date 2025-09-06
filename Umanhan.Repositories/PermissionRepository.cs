using Umanhan.Models;
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
