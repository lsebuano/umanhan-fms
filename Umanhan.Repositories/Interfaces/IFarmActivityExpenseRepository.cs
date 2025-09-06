using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmActivityExpenseRepository : IRepository<FarmActivityExpense>
    {
        // add new methods specific to this repository
        Task<List<FarmActivityExpense>> GetFarmActivityExpensesAsync(Guid farmId, string[] includeEntities);
        Task<List<FarmActivityExpense>> GetFarmActivityExpensesByActivityAsync(Guid activityId, string[] includeEntities);
    }
}
