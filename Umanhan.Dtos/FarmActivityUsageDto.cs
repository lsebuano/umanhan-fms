namespace Umanhan.Dtos
{
    public class FarmActivityUsageDto : BaseDto
    {
        public Guid UsageId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid InventoryId { get; set; }

        public Guid? UnitId { get; set; }

        public string ItemName { get; set; }

        public decimal UsageHours { get; set; }

        public decimal Rate { get; set; }

        public string Unit { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime Timestamp { get; set; }

        public void Recompute()
        {
            TotalCost = Rate * UsageHours;
        }
    }
}
