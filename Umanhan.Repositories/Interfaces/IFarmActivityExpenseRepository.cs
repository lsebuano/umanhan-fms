using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
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
