namespace Umanhan.Dtos
{
    public class InventoryDto
    {
        public Guid InventoryId { get; set; }

        public Guid CategoryId { get; set; }

        public Guid? UnitId { get; set; }

        public string ItemName { get; set; }

        public string Category { get; set; }

        public string Unit { get; set; }

        public FarmInventoryDto FarmInventory { get; set; }

        public IEnumerable<FarmActivityUsageDto> FarmActivityUsages { get; set; } = [];

    }
}
