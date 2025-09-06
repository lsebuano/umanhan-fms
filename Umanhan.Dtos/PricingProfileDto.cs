namespace Umanhan.Dtos
{
    public class PricingProfileDto
    {
        public Guid ProfileId { get; set; }

        public Guid FarmId { get; set; }

        public string FarmName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal FinalPrice { get; set; }

        public IEnumerable<PricingDto> PricingConditions { get; set; } = [];
    }
}
