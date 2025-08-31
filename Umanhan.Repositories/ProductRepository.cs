using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class ProductRepository : UmanhanRepository<VwProductLookup>, IProductRepository
    {
        public ProductRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public async Task<List<VwProductLookup>> GetProductsByFarmAsync(Guid farmId)
        {
            return await _context.VwProductLookups
                .Where(x => x.FarmId == farmId)
                .OrderBy(x => x.ProductName)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<List<VwProductLookup>> GetProductsByFarmByTypeAsync(Guid farmId, Guid typeId)
        {
            return await _context.VwProductLookups
                .Where(x => x.FarmId == farmId &&
                            x.ProductTypeId == typeId)
                .OrderBy(x => x.ProductName)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<VwProductLookup> GetProductByIdAsync(Guid typeId, Guid id)
        {
            return await _context.VwProductLookups
                .Where(x => x.ProductTypeId == typeId &&
                            x.ProductId == id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<Dictionary<ProductKey, ProductDto>> BuildProductLookupAsync()
        {
            var list = await _context.VwProductLookups
                .ToListAsync()
                .ConfigureAwait(false);

            var lookup = new Dictionary<ProductKey, ProductDto>();
            foreach (var p in list)
            {
                var key = new ProductKey(p.ProductId.Value, p.ProductTypeId.Value);
                var dto = new ProductDto
                {
                    ProductId = p.ProductId.Value,
                    ProductName = p.ProductName,
                    Variety = p.Variety,
                };
                lookup[key] = dto;
            }

            return lookup;
        }
    }
}
