using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmExpenseService> _logger;

        public FarmExpenseService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmExpenseService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        //public async Task<IEnumerable<FarmExpenseDto>> GetFarmExpensesAsync(DateOnly periodStart, DateOnly periodEnd)
        //{
        //    var list1 = await _unitOfWork.FarmActivityExpenses.GetAllAsync().ConfigureAwait(false);
        //    list1.Select(x => new FarmExpenseDto
        //    {

        //    });
        //}
    }
}
