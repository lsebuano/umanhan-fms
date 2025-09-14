namespace Umanhan.Dtos
{
    public class FarmProduceInventoryDto : BaseDto
    {
        public Guid InventoryId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductTypeId { get; set; }

        public decimal InitialQuantity { get; set; }

        public Guid UnitId { get; set; }

        public DateTime Date { get; set; }

        public decimal UnitPrice { get; set; }

        public string Notes { get; set; }

        public decimal CurrentQuantity { get; set; }

        public string ProductType { get; set; }
        
        public string Product { get; set; }

        public string ProductVariety { get; set; }

        public string Unit { get; set; }
    }
}
