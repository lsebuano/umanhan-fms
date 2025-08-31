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
    public class QuotationRepository : UmanhanRepository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task<List<Quotation>> GetQuotationsByFarmIdAsync(Guid farmId, params string[] includeEntities)
        {
            var query = _context.Quotations.AsQueryable().AsSplitQuery();
            if (includeEntities != null && includeEntities.Length > 0)
            {
                foreach (var entity in includeEntities)
                {
                    query = query.Include(entity);
                }
            }
            return query.Where(x => x.FarmId == farmId)
                .OrderByDescending(x => x.DateSent)
                .ToListAsync();
        }

        public Task<List<Quotation>> GetQuotationsByFarmIdAsync(Guid farmId, int topN, params string[] includeEntities)
        {
            var query = _context.Quotations.AsQueryable().AsSplitQuery();
            if (includeEntities != null && includeEntities.Length > 0)
            {
                foreach (var entity in includeEntities)
                {
                    query = query.Include(entity);
                }
            }
            return query.Where(x => x.FarmId == farmId)
                .OrderByDescending(x => x.DateSent)
                .Take(topN)
                .ToListAsync();
        }

        public Task UpdateQuotationMessageIdAsync(Quotation quotation)
        {
            _context.Quotations.Attach(quotation);
            _context.Entry(quotation).Property(x => x.AwsSesMessageId).IsModified = true;
            _context.Entry(quotation).Property(x => x.DateSent).IsModified = true;
            _context.Entry(quotation).Property(x => x.Status).IsModified = true;
            return Task.CompletedTask;
        }
    }
}
