namespace Umanhan.Dtos
{
    public class FarmSetupDto : BaseDto
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
    }
}
