using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class LaborerDto
    {
        public Guid LaborerId { get; set; }

        public string Name { get; set; }

        public string Skillset { get; set; }

        public string ContactInfo { get; set; }

        public decimal? DailyRate { get; set; }

        public decimal? ContractedRate { get; set; }

        public IEnumerable<FarmActivityLaborerDto> FarmActivityLaborers { get; set; } = [];
    }
}
