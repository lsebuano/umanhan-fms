using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Repositories.Interfaces
{
    public interface IQuotationRepository : IRepository<Quotation>
    {
        // add new methods specific to this repository
        Task<List<Quotation>> GetQuotationsByFarmIdAsync(Guid farmId, params string[] includeEntities);
        Task<List<Quotation>> GetQuotationsByFarmIdAsync(Guid farmId, int topN, params string[] includeEntities);
        Task UpdateQuotationMessageIdAsync(Quotation quotation);
    }
}