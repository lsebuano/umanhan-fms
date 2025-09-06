using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmGeneralExpenseRepository : IRepository<FarmGeneralExpense>
    {
        // add new methods specific to this repository
        Task<List<FarmGeneralExpense>> GetFarmGeneralExpensesAsync(Guid farmId, string[] includeEntities);
        Task<List<FarmGeneralExpense>> GetFarmGeneralExpensesAsync(Guid farmId, DateTime startDate, DateTime endDate, string[] includeEntities);
    }
}
