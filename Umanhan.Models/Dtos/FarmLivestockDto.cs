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
    public class FarmLivestockDto
    {
        public Guid FarmLivestockId { get; set; }

        public Guid FarmId { get; set; }

        public Guid LivestockId { get; set; }

        [JsonIgnore]
        public DateOnly? PurchaseDateUtc { get; set; }

        public DateOnly? PurchaseDate => PurchaseDateUtc?.ToManilaTime();

        [JsonIgnore]
        public DateOnly? BirthDateUtc { get; set; }
        
        public DateOnly? BirthDate => BirthDateUtc?.ToManilaTime();

        public string Breed { get; set; }

        public Guid ZoneId { get; set; }

        public int Quantity { get; set; }

        public decimal PurchaseCost { get; set; }

        public Guid UnitId { get; set; }

        public decimal DefaultRate { get; set; }

        public string FarmName { get; set; }

        public string FarmLocation { get; set; }

        public string AnimalType { get; set; }

        public string ZoneName { get; set; }

        public decimal ZoneSizeInHectares { get; set; }
    }
}
