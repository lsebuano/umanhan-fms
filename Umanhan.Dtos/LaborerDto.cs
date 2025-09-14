namespace Umanhan.Dtos
{
    public class LaborerDto : BaseDto
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
