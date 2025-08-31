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
    public class StaffRepository : UmanhanRepository<Staff>, IStaffRepository
    {
        public StaffRepository(UmanhanDbContext context) : base(context)
        {
        }

        // add new methods specific to this repository

        public async Task<List<Staff>> GetStaffsByFarmAsync(Guid farmId)
        {
            return await _context.Staffs.AsNoTracking()
                                        .Where(s => s.FarmId == farmId)
                                        .ToListAsync()
                                        .ConfigureAwait(false);
        }
    }
}
