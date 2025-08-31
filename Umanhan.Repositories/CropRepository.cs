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
    public class CropRepository: UmanhanRepository<Crop>, ICropRepository
    {
        public CropRepository(UmanhanDbContext context) : base(context)
        {
        }
    }
}
