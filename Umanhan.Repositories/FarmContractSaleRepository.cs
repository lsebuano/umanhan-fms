using Microsoft.EntityFrameworkCore;
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
    public class FarmContractSaleRepository : UmanhanRepository<FarmContractSale>, IFarmContractSaleRepository
    {
        public FarmContractSaleRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository
        public Task<List<FarmContractSale>> GetFarmContractSalesAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmContractSale> query = _context.FarmContractSales.AsNoTracking().AsSplitQuery().Include(x => x.ContractDetail.Contract);
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ContractDetail.Contract.FarmId == farmId)
                        .OrderBy(x => x.Date)
                        .ToListAsync();
        }

    }
}
