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

namespace Umanhan.Repositories
{
    public class FarmContractPaymentRepository : UmanhanRepository<FarmContractPayment>, IFarmContractPaymentRepository
    {
        public FarmContractPaymentRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<FarmContractPayment> GetAsync(string paymentId)
        {
            return _context.FarmContractPayments.AsNoTracking()
                                                .AsSplitQuery()
                                                .Include(x => x.FarmContractPaymentDetails)       
                                                .FirstOrDefaultAsync(x => x.InvoiceNo.Equals(paymentId));
        }
    }
}
