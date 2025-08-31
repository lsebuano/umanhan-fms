using Microsoft.EntityFrameworkCore;
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
    public class CustomerRepository : UmanhanRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public Task<List<Customer>> GetCustomersContractEligibleAsync(string[] includeEntities)
        {
            var query = _context.Customers.AsQueryable().Where(c => c.ContractEligible);
            if (includeEntities.Any())
            {
                foreach (var includeEntity in includeEntities)
                {
                    query = query.Include(includeEntity);
                }
            }
            return query.AsSplitQuery()
                        .AsNoTracking()
                        .ToListAsync();
        }

        public Task<List<Customer>> GetCustomersByTypeAsync(Guid customerTypeId, string[] includeEntities)
        {
            var query = _context.Customers.AsQueryable().Where(c => c.CustomerTypeId == customerTypeId);
            if (includeEntities.Any())
            {
                foreach (var includeEntity in includeEntities)
                {
                    query = query.Include(includeEntity);
                }
            }
            return query.AsSplitQuery()
                        .AsNoTracking()
                        .ToListAsync();
        }
    }
}
