using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
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
