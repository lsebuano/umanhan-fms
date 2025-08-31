using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class StaffDto
    {
        public Guid StaffId { get; set; }

        public Guid FarmId { get; set; }

        public string Name { get; set; }

        public string ContactInfo { get; set; }

        [JsonIgnore]
        public DateOnly? HireDateUtc { get; set; }

        public DateOnly? HireDate => HireDateUtc?.ToManilaTime();

        public string Status { get; set; }

        public FarmDto Farm { get; set; }

        public IEnumerable<FarmActivityDto> FarmActivities { get; set; } = [];
    }
}
