using Microsoft.EntityFrameworkCore;
using Umanhan.Dtos;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class CategoryRepository : UmanhanRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<CategoryDto>> GetCategoriesByGroup2Async(string group2)
        {
            return _context.Categories.AsNoTracking()
                                      .Where(x => x.Group2.Equals(group2))
                                      .Select(x => new CategoryDto
                                      {
                                          CategoryId = x.Id,
                                          CategoryName = x.CategoryName,
                                          Group = x.Group,
                                          Group2 = x.Group2,
                                          ConsumptionBehavior = x.ConsumptionBehavior
                                      })
                                      .ToListAsync();
        }

        public Task<List<CategoryDto>> GetCategoriesByGroupAsync(string group)
        {
            return _context.Categories.AsNoTracking()
                                      .Where(x => x.Group.Equals(group))
                                      .Select(x => new CategoryDto
                                      {
                                          CategoryId = x.Id,
                                          CategoryName = x.CategoryName,
                                          Group = x.Group,
                                          Group2 = x.Group2,
                                          ConsumptionBehavior = x.ConsumptionBehavior
                                      })
                                      .ToListAsync();
        }

        public Task<List<CategoryDto>> GetCategoriesByConsumptionBehaviorAsync(string consumptionBehavior)
        {
            return _context.Categories.AsNoTracking()
                                      .Where(x => x.ConsumptionBehavior.Equals(consumptionBehavior))
                                      .Select(x => new CategoryDto
                                      {
                                          CategoryId = x.Id,
                                          CategoryName = x.CategoryName,
                                          Group = x.Group,
                                          Group2 = x.Group2,
                                          ConsumptionBehavior = x.ConsumptionBehavior
                                      })
                                      .ToListAsync();
        }
    }
}
