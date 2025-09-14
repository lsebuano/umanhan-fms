namespace Umanhan.Dtos
{
    public class ProductDto : BaseDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductTypeId { get; set; }

        public string ProductName { get; set; }

        public string Variety { get; set; }

        public string ProductTypeName { get; set; }

        public Guid FarmId { get; set; }

        public decimal DefaultRate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EstimatedHarvestDate { get; set; }

        public string FarmName { get; set; }

        public Guid UnitId { get; set; }

        public string UnitName { get; set; }

        public Guid ZoneId { get; set; }

        public string ZoneName { get; set; }
    }

    // Key for the lookup
    public record ProductKey(Guid ProductId, Guid ProductTypeId);
}
