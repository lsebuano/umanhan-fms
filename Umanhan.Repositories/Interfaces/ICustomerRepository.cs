using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        // add new methods specific to this repository
        Task<List<Customer>> GetCustomersContractEligibleAsync(string[] includeEntities);
        Task<List<Customer>> GetCustomersByTypeAsync(Guid customerTypeId, string[] includeEntities);
    }
}
