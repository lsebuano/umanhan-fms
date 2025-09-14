namespace Umanhan.Dtos
{
    public class FarmDto : BaseDto
    {
        public Guid FarmId { get; set; }

        public string FarmName { get; set; }

        public string Location { get; set; }

        public string FullAddress { get; set; }

        public decimal? SizeInHectares { get; set; }

        public decimal? SizeInSqm { get; set; }

        public string OwnerName { get; set; }
        public string Tin { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }

        public string Notes { get; set; }

        public decimal? Lng { get; set; }

        public decimal? Lat { get; set; }

        public string BoundaryJson { get; set; }

        public bool SetupComplete { get; set; }

        public string SetupCompleteString => SetupComplete ? "Yes" : "No";

        public bool SetupStarted { get; set; }

        public string SetupStartedString => SetupStarted ? "Yes" : "No";

        public string StaticMapUrl { get; set; }

        public IEnumerable<FarmActivityDto> FarmActivities { get; set; } = [];

        public IEnumerable<FarmContractDto> FarmContracts { get; set; } = [];

        public IEnumerable<FarmCropDto> FarmCrops { get; set; } = [];

        public IEnumerable<FarmInventoryDto> FarmInventories { get; set; } = [];

        public IEnumerable<FarmLivestockDto> FarmLivestocks { get; set; } = [];

        public IEnumerable<FarmZoneDto> FarmZones { get; set; } = [];

        public IEnumerable<StaffDto> Staffs { get; set; } = [];
    }
}
