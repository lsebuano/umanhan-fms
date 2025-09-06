using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmActivityRepository : IRepository<FarmActivity>
    {
        // add new methods specific to this repository
        Task<List<FarmActivity>> GetFarmActivitiesAsync(Guid farmId);
        Task<List<FarmActivity>> GetFarmActivitiesAsync(Guid farmId, DateTime date, params string[] includeEntities);
        Task<List<FarmActivityExpense>> GetFarmActivityExpensesAsync(Guid activityId, params string[] includeEntities);
    }
}
