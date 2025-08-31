using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class FarmNumberSeryRepository : UmanhanRepository<FarmNumberSery>, IFarmNumberSeryRepository
    {
        public FarmNumberSeryRepository(UmanhanDbContext context) : base(context)
        {
        }

        public async Task<string> GenerateNumberSeryAsync(Guid farmId, string type)
        {
            // use dapper!
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var result = await conn.QueryFirstOrDefaultAsync<string>(
                "SELECT fn_generate_farm_number_series(@p_farm_id,@p_type)",
                new
                {
                    p_farm_id = farmId,
                    p_type = type
                }
            );
            return result;
        }
    }
}
