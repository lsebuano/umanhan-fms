using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class FarmGeneralExpenseReceiptRepository : UmanhanRepository<FarmGeneralExpenseReceipt>, IFarmGeneralExpenseReceiptRepository
    {
        public FarmGeneralExpenseReceiptRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository
        public Task<List<FarmGeneralExpenseReceipt>> GetFarmGeneralExpenseReceiptsByGeneralExpenseAsync(Guid generalExpenseId, params string[] includeEntities)
        {
            IQueryable<FarmGeneralExpenseReceipt> query = _context.FarmGeneralExpenseReceipts.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.GeneralExpenseId == generalExpenseId)
                        .ToListAsync();
        }
    }
}
