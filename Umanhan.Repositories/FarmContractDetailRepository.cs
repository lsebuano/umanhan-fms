using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class FarmContractDetailRepository : UmanhanRepository<FarmContractDetail>, IFarmContractDetailRepository
    {
        public FarmContractDetailRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmContractDetail>> GetFarmContractDetailsAsync(Guid contractId, string[] includeEntities)
        {
            IQueryable<FarmContractDetail> query = _context.FarmContractDetails.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.ContractId == contractId)
                        .ToListAsync();
        }

        public Task<List<FarmContractDetail>> GetFarmContractDetailsAsync(Guid farmId, DateTime dateStart, DateTime dateEnd)
        {
            DateOnly ds = DateOnly.FromDateTime(dateStart);
            DateOnly de = DateOnly.FromDateTime(dateEnd);

            return _context.FarmContractDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Contract)
                .Where(x => x.Contract.FarmId == farmId && 
                            x.Contract.ContractDate >= ds && 
                            x.Contract.ContractDate <= de)
                .ToListAsync();
        }
    }
}
