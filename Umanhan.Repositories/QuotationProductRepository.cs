using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Repositories
{
    public class QuotationProductRepository : UmanhanRepository<QuotationProduct>, IQuotationProductRepository
    {
        public QuotationProductRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
