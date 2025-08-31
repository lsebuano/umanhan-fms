using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Umanhan.Repositories
{
    public class FarmContractRepository : UmanhanRepository<FarmContract>, IFarmContractRepository
    {
        public FarmContractRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<FarmContract>> GetFarmContractsAsync(Guid farmId, string[] includeEntities)
        {
            IQueryable<FarmContract> query = _context.FarmContracts.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId)
                        .OrderBy(x => x.ContractDate)
                        .ToListAsync();
        }

        public Task<List<FarmContract>> GetFarmContractsAsync(Guid farmId, DateTime startDate, DateTime endDate, string[] includeEntities)
        {
            var ds = DateOnly.FromDateTime(startDate.Date);
            var de = DateOnly.FromDateTime(endDate.Date).AddDays(1);

            IQueryable<FarmContract> query = _context.FarmContracts.AsNoTracking().AsSplitQuery();
            foreach (var includeEntity in includeEntities)
            {
                query = query.Include(includeEntity);
            }
            return query.Where(x => x.FarmId == farmId &&
                                    x.ContractDate >= ds &&
                                    x.ContractDate <= de)
                        .OrderBy(x => x.ContractDate)
                        .ToListAsync();
        }

        public Dictionary<ProductKey, ProductDto> BuildProductLookup()
        {
            var productTypes = _context.ProductTypes
                .AsNoTracking()
                .ToDictionary(pt => pt.Id);

            var cropDict = _context.Crops
                .AsNoTracking()
                .ToDictionary(c => c.Id, c => new ProductDto
                {
                    ProductId = c.Id,
                    ProductName = c.CropName
                });

            var livestockDict = _context.Livestocks
                .AsNoTracking()
                .ToDictionary(l => l.Id, l => new ProductDto
                {
                    ProductId = l.Id,
                    ProductName = l.AnimalType
                });

            var lookup = new Dictionary<ProductKey, ProductDto>();
            foreach (var pt in productTypes.Values)
            {
                var sourceDict = pt.TableName switch
                {
                    "crops" => cropDict,
                    "livestocks" => livestockDict,
                    _ => throw new InvalidOperationException(
                           $"Unknown table: {pt.TableName}")
                };

                // for EVERY key in that source, add a lookup entry
                foreach (var kvp in sourceDict)
                {
                    var productId = kvp.Key;
                    var dto = kvp.Value;

                    // key is the pair (ProductId, ProductTypeId)
                    var key = new ProductKey(productId, pt.Id);
                    lookup[key] = dto;
                }
            }
            return lookup;
        }
    }
}
