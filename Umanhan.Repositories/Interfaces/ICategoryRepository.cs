using Umanhan.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface ICategoryRepository: IRepository<Category>
    {
        // add new methods specific to this repository
        Task<List<CategoryDto>> GetCategoriesByGroupAsync(string group);
        Task<List<CategoryDto>> GetCategoriesByGroup2Async(string group);
        Task<List<CategoryDto>> GetCategoriesByConsumptionBehaviorAsync(string consumptionBehavior);
    }
}
