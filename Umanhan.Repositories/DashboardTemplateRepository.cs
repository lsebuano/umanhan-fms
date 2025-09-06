using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class DashboardTemplateRepository : UmanhanRepository<DashboardTemplate>, IDashboardTemplateRepository
    {
        public DashboardTemplateRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
