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
    public class QuotationDetailRepository : UmanhanRepository<QuotationDetail>, IQuotationDetailRepository
    {
        public QuotationDetailRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
