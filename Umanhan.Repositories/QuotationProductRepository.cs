using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class QuotationProductRepository : UmanhanRepository<QuotationProduct>, IQuotationProductRepository
    {
        public QuotationProductRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
