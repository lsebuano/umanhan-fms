using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IFarmContractPaymentRepository : IRepository<FarmContractPayment>
    {
        // add new methods specific to this repository
        Task<FarmContractPayment> GetAsync(string paymentId);
    }
}
