using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class QuotationDetailRepository : UmanhanRepository<QuotationDetail>, IQuotationDetailRepository
    {
        public QuotationDetailRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
