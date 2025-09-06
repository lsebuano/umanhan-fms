using System.Text.Json.Serialization;
using Umanhan.Shared;

namespace Umanhan.Dtos
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
