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
    public class FarmCropDto
    {
        public Guid FarmCropId { get; set; }

        public Guid FarmId { get; set; }

        public Guid CropId { get; set; }

        public Guid UnitId { get; set; }

        public Guid ZoneId { get; set; }

        public decimal DefaultRate { get; set; }

        //[JsonIgnore]
        public DateOnly? PlantingDateUtc { get; set; }

        public DateOnly? PlantingDate => PlantingDateUtc?.ToManilaTime();

        //[JsonIgnore]
        public DateOnly? EstimatedHarvestDateUtc { get; set; }
        
        public DateOnly? EstimatedHarvestDate => EstimatedHarvestDateUtc?.ToManilaTime();

        public string CropName { get; set; }

        public string CropVariety { get; set; }

        public string CropUnit { get; set; }

        public string FarmName { get; set; }

        public string FarmLocation { get; set; }

        public string ZoneName { get; set; }

        public decimal? ZoneSizeInHectares { get; set; }

        public string ZoneIrrigationType { get; set; }
        public string ZoneSoilType { get; set; }
    }
}
