namespace Umanhan.Dtos
{
    public class FarmInventoryDto
    {
        public Guid FarmInventoryId { get; set; }

        public Guid FarmId { get; set; }

        public Guid InventoryId { get; set; }

        public Guid UnitId { get; set; }

        public decimal Quantity { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }

        public string FarmName { get; set; }

        public string FarmLocation { get; set; }

        public string InventoryItemName { get; set; }

        public string InventoryItemImageThumbnail { get; set; }

        public string InventoryItemImageFull { get; set; }

        public string InventoryItemImageContentType { get; set; }

        public string InventoryItemImageS3UrlThumbnail { get; set; }

        public string InventoryItemImageS3UrlFull { get; set; }

        public string InventoryCategory { get; set; }

        public string InventoryUnit { get; set; }
        public string InventoryCategoryGroup { get; set; }
        public string InventoryCategoryGroup2 { get; set; }
        public string InventoryCategoryConsumptionBehavior { get; set; }
    }
}
