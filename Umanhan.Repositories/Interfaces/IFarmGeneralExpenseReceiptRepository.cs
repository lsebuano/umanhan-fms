using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmGeneralExpenseReceiptRepository : IRepository<FarmGeneralExpenseReceipt>
    {
        // add new methods specific to this repository
        Task<List<FarmGeneralExpenseReceipt>> GetFarmGeneralExpenseReceiptsByGeneralExpenseAsync(Guid genralExpenseId, params string[] includeEntities);
    }
}
