using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class LivestockDto
    {
        public Guid LivestockId { get; set; }

        public string AnimalType { get; set; }

        public string Breed { get; set; }

        public IEnumerable<FarmLivestockDto> FarmLivestocks { get; set; } = [];
    }
}
