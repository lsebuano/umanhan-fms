using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class ModuleRepository : UmanhanRepository<Module>, IModuleRepository
    {
        public ModuleRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
