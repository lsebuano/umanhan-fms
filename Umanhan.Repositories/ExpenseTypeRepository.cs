using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class ExpenseTypeRepository : UmanhanRepository<ExpenseType>, IExpenseTypeRepository
    {
        public ExpenseTypeRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

    }
}
