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
    public class FarmProduceInventoryRepository : UmanhanRepository<FarmProduceInventory>, IFarmProduceInventoryRepository
    {
        public FarmProduceInventoryRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmProduceInventory>> GetFarmProduceInventoriesAsync(Guid farmId, params string[] includeEntities)
        {
            IQueryable<FarmProduceInventory> query = _context.FarmProduceInventories.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId)
                        .OrderBy(x => x.Date)
                        .ToListAsync();
        }

        public Task<List<FarmProduceInventory>> GetFarmProduceInventoriesAsync(Guid farmId, Guid typeId, params string[] includeEntities)
        {
            IQueryable<FarmProduceInventory> query = _context.FarmProduceInventories.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId &&
                                    x.ProductTypeId == typeId)
                        .OrderBy(x => x.Date)
                        .ToListAsync();
        }
    }
}
