namespace Umanhan.Dtos
{
    public class FarmZoneYieldDto
    {
        public Guid YieldId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ZoneId { get; set; }

        public Guid ProductTypeId { get; set; }

        public Guid ProductId { get; set; }

        public Guid UnitId { get; set; }

        public Guid ContractDetailId { get; set; }

        public decimal ExpectedYield { get; set; }

        public decimal? ActualYield { get; set; }

        public decimal? ForecastedYield { get; set; }

        public string UnitName { get; set; }

        public string ProductTypeName { get; set; }

        public string ProductName { get; set; }

        public string ZoneName { get; set; }

        public DateOnly HarvestDate { get; set; }
    }
}
