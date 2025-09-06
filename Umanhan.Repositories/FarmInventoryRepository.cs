using Microsoft.EntityFrameworkCore;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class FarmInventoryRepository : UmanhanRepository<FarmInventory>, IFarmInventoryRepository
    {
        public FarmInventoryRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmInventory>> GetFarmInventoriesAsync(Guid farmId, params string[] includeEntities)
        {
            var query = _dbSet.AsQueryable();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.AsNoTracking()
                        .AsSplitQuery()
                        .Where(x => x.FarmId == farmId)
                        .ToListAsync();
        }

        public Task<FarmInventory> GetFarmInventoryByIdAsync(Guid farmId, Guid itemId, params string[] includeEntities)
        {
            var query = _dbSet.AsQueryable();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.AsNoTracking()
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(x => x.FarmId == farmId &&
                                                  x.InventoryId == itemId);
        }

        public void UpdateQuantity(Guid inventoryId, decimal newValue)
        {
            var obj = new FarmInventory { Id = inventoryId };
            _context.Attach(obj);

            var entry = _context.Entry(obj);
            entry.Property(u => u.Quantity).CurrentValue = newValue;
            entry.Property(u => u.Quantity).IsModified = true;
        }
    }
}
