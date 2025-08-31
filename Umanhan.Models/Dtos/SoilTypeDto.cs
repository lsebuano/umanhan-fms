using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class SoilTypeDto
    {
        public Guid SoilId { get; set; }

        public string SoilName { get; set; }

        public string SoilPh { get; set; }

        public string SoilOrganicCarbon { get; set; }

        public string SoilMoisture { get; set; }

        public string SoilFertility { get; set; }

        public string Notes { get; set; }

        public IEnumerable<FarmZoneDto> FarmZones { get; set; } = [];
    }
}
